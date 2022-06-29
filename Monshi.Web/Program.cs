using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Monshi.ApplicationService;
using Monshi.Data.SqlServer;
using Monshi.Domain;
using Monshi.Domain.Logs;
using Monshi.Domain.Products;
using Monshi.Domain.Users;
using Monshi.Web.Resources;
using WebApplication2.Elmah;
using WebApplication2.Filters;
using WebApplication2.Hangfire;
using WebApplication2.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

var cultures = new List<CultureInfo>();
cultures.Add(new CultureInfo("fa"));
cultures.Add(new CultureInfo("en"));
builder.Services.AddRequestLocalization(options =>
{
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
    options.DefaultRequestCulture = new RequestCulture("fa");
});

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
    {
        //options.Filters.Add(new MyAuthorize());
        //options.Filters.Add(typeof(LoggerAttribute));
        options.CacheProfiles.Add(new KeyValuePair<string, CacheProfile>("c1", new CacheProfile()
        {
            Duration = 10,
            Location = ResponseCacheLocation.Any
        }));

    })
    //.AddMvcLocalization(options=>options.ResourcesPath="Resources")
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            return factory.Create(typeof(SharedResource));
        };
    })
    .AddRazorRuntimeCompilation();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
builder.Services.AddScoped<SmsService>();
builder.Services.AddDbContext<ApplicationDbContext>(config =>
{
    config.UseSqlServer(builder.Configuration.GetConnectionString("default"));
});
/*
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents()
        {
            OnTokenValidated = async context =>
            {
                var userId = int.Parse(context.HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId).Value);
                var serial = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "SerialNo").Value;
                var userService = context.HttpContext.RequestServices.GetService<UserService>();

                var token = context.Request.Headers["Authorization"].ToString();
                token = token.Replace("Bearer ", "");
                var user = await userService.FindUserAsync(userId);
                if (user.SerialNo != serial)
                    context.Fail("Serial Number Has been Changed");
            }
        };
    });
*/
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/auth/login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        options.SlidingExpiration = true;
        options.Events = new CookieAuthenticationEvents()
        {
            OnValidatePrincipal = async context =>
            {
                var claim = context.Principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier);
                var serialNo = context.Principal.Claims.First(x => x.Type == ClaimTypes.SerialNumber).Value;
                var userId = int.Parse(claim.Value);
              
                var userService = context.HttpContext.RequestServices.GetService<IUserService>();
                var user = await userService.FindUserAsync(userId);
                //if (user.IsAdmin == false)
                //context.RejectPrincipal();
                if (serialNo != user.SerialNo)
                {
                    context.RejectPrincipal();
                }
            }
        };

    });

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("hangfire"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();
builder.Services.AddElmah<SqlErrorLog>(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("elmah");
    //options.Path="/elmah";
    options.LogPath = "~/logs";
    //options.OnPermissionCheck = context => context.User.Identity.IsAuthenticated && context.User.IsInRole("admin");
    var apiKey = builder.Configuration["sms:ghasedak:apikey"];
    var mobile = builder.Configuration["elmah:mobile"];
    
    options.Notifiers.Add(new ElmahNotifier(apiKey,mobile));
    options.Filters.Add(new ElmahError404());
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

/*using (var scope=app.Services.CreateAsyncScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await databaseInitializer.SeedAsync();
}*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseElmahExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.UseElmah();

/*var options = new RequestLocalizationOptions();
options.RequestCultureProviders.Insert(0,new CustomLocalizationProvider());
app.UseRequestLocalization(options);*/

app.UseRequestLocalization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard();
});
app.UseHangfireDashboard(options:new DashboardOptions()
{
    Authorization = new []{new HangfireAuthorization()}
});


var licensePath = Path.Combine(builder.Environment.ContentRootPath, "Reports", "license.key");
//Stimulsoft.Base.StiLicense.LoadFromFile(licensePath);
app.Run();
