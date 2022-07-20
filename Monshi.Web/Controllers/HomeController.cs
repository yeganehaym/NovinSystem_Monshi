using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Monshi.Data.SqlServer;
using Monshi.Domain.Logs;
using Monshi.Domain.Users.Entities;
using Monshi.Web.Models;
using Newtonsoft.Json;
using RestSharp;
using WebApplication2.Filters;
using WebApplication2.RestSharp;

namespace Monshi.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private ApplicationDbContext _dbContext;
    private IMemoryCache _memoryCache;
    private IStringLocalizer<HomeController> _stringLocalizer;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, IMemoryCache memoryCache, IStringLocalizer<HomeController> stringLocalizer)
    {
        _logger = logger;
        _dbContext = dbContext;
        _memoryCache = memoryCache;
        _stringLocalizer = stringLocalizer;
    }

    
    //[TypeFilter(typeof(Log))]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }

    public IActionResult GetUsers()
    {
        var users = _dbContext
            .Users
            //.IgnoreQueryFilters()
            .ToList();

        var items = users
            .Select(x => new {x.Username, x.FirstName, x.LastName})
            .ToList();

        return Json(items);
    }

    [AllowAnonymous]
    public async Task<IActionResult> UpdateUser(int id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        user.ModateHozoor=TimeSpan.FromHours(4);
        await _dbContext.SaveChangesAsync();
        return new EmptyResult();
    }
    public async Task<IActionResult> AdUser()
    {
        await _dbContext.Users.AddAsync(new User()
        {
            Username = "hassan",
            Password = "123456",
            FirstName = "haasan",
            LastName = "hassani",
            MobileNumber = "09365437062",
            ActivationStatus = ActivationStatus.Active,
            IsAdmin = false,
            SerialNo = "ded44444444444fgf"
        });
        await _dbContext.SaveChangesAsync();
        return new EmptyResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetOccasion()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> GetOccasion(OccationViewModel model)
    {
        Occasion occations = null;
        var result = _memoryCache.TryGetValue(model.Month + "-" + model.Day, out occations);

        
        if (!result)
        {
            var url = $"https://farsicalendar.com/api/sh/{model.Day}/{model.Month}";
            var client = new RestClient();
            var request = new RestRequest(url,Method.Get);
            var response=await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                var json = response.Content;
                occations = JsonConvert.DeserializeObject<Occasion>(json);
               
            }

            _memoryCache.Set(model.Month + "-" + model.Day, occations);
        }
        
        model.Occasions = new List<string>();
        foreach (var value in occations.values)
        {
            model.Occasions.Add(value.occasion + (value.dayoff?"(تعطیل)":""));
        }
        return View(model);
    }

    public async Task<IActionResult> SendSms(string number, string p)
    {
        var template = "RegtisterCode";
        var url = "/verification/send/simple";
        var client = SmsClient.GetClient();
        var request = SmsClient.GetRequest(url);
        request.Method = Method.Post;
        request.AddParameter("receptor", number);
        request.AddParameter("type", 1);
        request.AddParameter("template", template);
        request.AddParameter("param1", p);

        var response = await client.ExecuteAsync<GhasedakResult>(request);

        if (response.Data.result.code == 200)
        {
            var smsCode = response.Data.items.First();
            return Content("Code :" + smsCode);
        }

        return Content(response.Data.result.message);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetTime()
    {
        return View();
    }
    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(CacheProfileName = "c1")]
    public IActionResult GetTime2()
    {        
        var time = DateTime.Now.ToString("HH:mm:ss");

        return Content("Time=" + time);
    }
    
    [AllowAnonymous]
    public IActionResult ShowErrorMessage()
    {
        var error = _stringLocalizer["Error"].Value;
        return Content(error);
    }
    
    [AllowAnonymous]
    public IActionResult language()
    {
        return View();
    }

    public IActionResult ChangeLanguage(string lang)
    {
        var name = CookieRequestCultureProvider.DefaultCookieName;
        var value = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang));
        Response.Cookies.Append(name,value);
        return RedirectToAction("language");
    }

    [AllowAnonymous]
    public IActionResult TestQuery()
    {
        var list = new[] {new SqlParameter("@ispaid", true)};
        var p = true;
      //  _dbContext.Database.ExecuteSqlRaw("update factors set ispaid=@p1",list);
       // _dbContext.Database.ExecuteSqlInterpolated($"exec sp");

       var rows= _dbContext.Database.ExecuteSqlInterpolated($"update factors set ispaid={p}");
       return Content("Rows = " + rows);
    }

    [AllowAnonymous]
    public IActionResult TestQuery2()
    {
        var data = _dbContext
            .FactorQuery
            .FromSqlRaw("select * from myview")
            .ToList();

        return Json(data);
    }
}