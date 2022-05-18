using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Monshi.Data.SqlServer;
using Monshi.Domain.Logs;
using Newtonsoft.Json;

namespace WebApplication2.Filters;

public class LoggerAttribute:Attribute,IAsyncActionFilter
{
    private ApplicationDbContext _applicationDbContext;

    public LoggerAttribute(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }


    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var p = "";
        foreach (var query in context.HttpContext.Request.Query)
        {
            p += query.Key + ":" + query.Value + "-";
        }
        
        foreach (var key in context.HttpContext.Request.RouteValues.Keys)
        {
            if(key=="controller" || key=="action")
                continue;
                
            p += $"{key}:{context.HttpContext.Request.RouteValues[key].ToString()} ** ";
        }
        var args = JsonConvert.SerializeObject(context.ActionArguments);
        p += args;

        var log = new Log()
        {
            Controller = context.HttpContext.Request.RouteValues["controller"].ToString(),
            Action = context.HttpContext.Request.RouteValues["action"].ToString(),
            Ip = context.HttpContext.Connection.RemoteIpAddress.ToString(),
            Paramters = p,
            UserId = context.HttpContext.User.GetUserId()
        };
        await _applicationDbContext.Logs.AddAsync(log);
        await _applicationDbContext.SaveChangesAsync();
        await next();
    }
}