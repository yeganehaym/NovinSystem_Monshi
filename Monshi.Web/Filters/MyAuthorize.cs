using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication2.Filters;

public class MyAuthorize:Attribute,IAsyncActionFilter
{
    public string Roles { get; set; }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.User.Identity.IsAuthenticated)
        {
            if (Roles == null || Roles == "")
                next();
            var myroles = Roles.Split(",");
            var claimRoles = context.HttpContext.User
                .Claims.Where(x => x.Type == ClaimTypes.Role)
                .ToList();
            foreach (var claimRole in claimRoles)
            {
                if (myroles.Contains(claimRole.Value))
                {
                    await next();
                    break;
                }
            }
        }
    }
}