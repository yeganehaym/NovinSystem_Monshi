using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Mvc;
using Monshi.Data.SqlServer;
using Monshi.Domain.Orders;
using Monshi.Domain.Products.Entities;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using WebApplication2.ModelBinders;

namespace Monshi.Web.Controllers;

public class ReportController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult LoadReportData()
    {
        var report = new StiReport();
        report.Load(StiNetCoreHelper.MapPath(this, "Reports/Report.mrt"));

        report.RegBusinessObject("Factor", new
        {
            Number="14010325",
            Date="1401/03/10",
            Price=120000,
            Discount=0
        });

        report.RegBusinessObject("Items", new[]
        {
            new {Name = "مانیتور 17 اینچ", Type = true, Price = 5000,Quantity=1, Discount = 0},
            new {Name = "ماوس فراسو", Type = false, Price = 10000,Quantity=5, Discount = 0},
            new {Name = "خودرو تیبا", Type = true, Price = 65000,Quantity=1, Discount = 0},
        });
        report["Today"] = DateTime.Now.ToShortPersianDateString();

        return StiNetCoreViewer.GetReportResult(this, report);
    }
    public IActionResult ViewerEvent()
    {
        return  StiNetCoreViewer.ViewerEventResult(this);
    }

    
    //public IActionResult TestBinder([ModelBinder(typeof(YeKeModelBinder))]string name)
    public IActionResult TestBinder(string name)
    {
        return Content(name);
    }

    
    
    
    
}