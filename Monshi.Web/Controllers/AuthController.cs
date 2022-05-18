
using System.Security.Claims;
using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Monshi.Data.SqlServer;
using Monshi.Domain.Users;
using Monshi.Domain.Users.Entities;
using Monshi.Web.Models;
using WebApplication2;
using WebApplication2.Messages;

namespace Monshi.Web.Controllers;

public class AuthController : Controller
{
    private IUserService _userService;
    private ApplicationDbContext _context;
    private SmsService _smsService;

    public AuthController(IUserService userService, ApplicationDbContext context, SmsService smsService)
    {
        _userService = userService;
        _context = context;
        _smsService = smsService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userService.FindUserAsync(model.Username, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("username","کاربری با این مشخصات یافت نگردید");
                return View(model);
            }
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name,user.Username));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName,user.FirstName + " " + user.LastName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.SerialNumber,user.SerialNo));
            
            if(user.IsAdmin)
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role,"admin"));
            
            var prinicipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prinicipal);

            return RedirectToAction("Index", "Home");
        }

        return View(model);
    }
    
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
   
    [HttpPost]
    public async Task<IActionResult> Register(RegisterPost model)
    {
        if (ModelState.IsValid)
        {
            var user =await _userService.FindUserAsync(model.Username);
            if (user!=null && user.ActivationStatus!=ActivationStatus.None)
            {
                ModelState.AddModelError("Username","این شماره قبلا ثبت شده است");
                return View(model);
            }
            else if (user==null)
            {
                user = new User()
                {
                    Username = model.Username,
                    MobileNumber = model.Username,
                    Password = model.Password,
                    SerialNo = Utils.RandomString(Utils.RandomType.All,10)
                };
                await _userService.NewUserAsync(user);
            }
         
            var otp = new OtpCode()
            {
                User = user,
                Code = Utils.RandomString(Utils.RandomType.Numbers,6)

            };
        
            await _userService.NewOtpCodeAsync(otp);
            var rows=await _context.SaveChangesAsync();
            if (rows > 0)
            {
                BackgroundJob.Enqueue(() => SendSms(user.MobileNumber,otp.Code));
                return RedirectToAction("CheckOTPCode");
            }
        }
        return View(model);
    }
    public async Task SendSms(string mobile,string otpCode)
    {
        var ghasedak = _smsService.GetSmsManger(0);
        await ghasedak.SendOtpMessage(mobile, "RegtisterCode", new[] {otpCode});
    }
    
    public IActionResult CheckOTPCode()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CheckOTPCode(OtpCodePost model)
    {
        if (ModelState.IsValid)
        {
            var otp =await _userService.GetOtpCode(model.Code);
         
            if (otp == null || otp.IsValid == false)
            {
                ModelState.AddModelError("Code","کد واردشده معتبر نمیباشد");
                return View(model);
            }

            otp.User.ActivationStatus = ActivationStatus.Active;
            otp.IsUsed = true;
            var row =await _context.SaveChangesAsync();
            if (row > 0)
                return RedirectToAction("Login");
        }
      
        return View(model);
    }

    
}