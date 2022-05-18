using System.Reflection;
using System.Security.Claims;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Monshi.ApplicationService;
using Monshi.Data.SqlServer;
using Monshi.Domain;
using Monshi.Domain.Logs;
using Monshi.Domain.Products;
using Monshi.Domain.Users;
using WebApplication2.Elmah;
using WebApplication2.Filters;
using WebApplication2.Hangfire;
using WebApplication2.Messages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    //options.Filters.Add(new MyAuthorize());
    options.Filters.Add(typeof(LoggerAttribute));
});
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
builder.Services.AddScoped<SmsService>();
builder.Services.AddDbContext<ApplicationDbContext>(config =>
{
    config.UseSqlServer(builder.Configuration.GetConnectionString("default"));
});



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
              
                var userService = context.HttpContext.RequestServices.GetService<UserService>();
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

var app = builder.Build();

using (var scope=app.Services.CreateAsyncScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await databaseInitializer.SeedAsync();
}

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
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseElmah();

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

app.Run();