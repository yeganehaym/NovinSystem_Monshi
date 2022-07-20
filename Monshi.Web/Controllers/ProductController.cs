using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monshi.Data.SqlServer;
using Monshi.Domain.Products;
using Monshi.Domain.Products.Entities;
using Monshi.Domain.Users;
using Monshi.Web.Models.DataTables;
using Monshi.Web.Models.Product;
using Parbad;
using Parbad.Gateway.ZarinPal;
using ZarinPal.Class;

namespace Monshi.Web.Controllers;

public class ProductController : Controller
{
    private ApplicationDbContext _applicationDbContext;
    private IUserService _userService;
    private IProductService _productService;
    private IOnlinePayment _onlinePayment;
    

    public ProductController(ApplicationDbContext applicationDbContext, IUserService userService, IProductService productService, IOnlinePayment onlinePayment)
    {
        _applicationDbContext = applicationDbContext;
        _userService = userService;
        _productService = productService;
        _onlinePayment = onlinePayment;
    }

    [HttpGet]
    public IActionResult NewProduct()
    {
        ViewBag.Dialog = true;
        return View(new AddNewProduct());
    }
    [HttpPost]
    public async Task<IActionResult> NewProduct(AddNewProduct addNewProduct)
    {
        var product = new Product()
        {
            Name = addNewProduct.Name,
            Price = addNewProduct.Price,
            ProductType = addNewProduct.ProductType,
            Quantity = addNewProduct.Quantity
        };
        await _productService.NewProductAsync(product);
        var rows = await _applicationDbContext.SaveChangesAsync();
        if (rows > 0)
        {
            TempData["Message"] = "محصول مورد نظر اضافه شد";
            return RedirectToAction("NewProduct");
        }

        addNewProduct.Message = "خطا در ثبت محصول";
        return View(addNewProduct);
    }

    public IActionResult GetListProducts()
    {
        return View();
    }

    public async Task<IActionResult> LoadProducts(DataTableParameters dataTableParameters, int productType)
    {
        var search = Request.Query["search[value]"].ToString();
        var products = await _productService.GetProductsAsync(dataTableParameters.Start, dataTableParameters.Length, search, productType);
        var totalCount = await _productService.GetProductsCountAsync(null);
        var filterCount = totalCount;
        if (string.IsNullOrEmpty(search))
        {
            filterCount = await _productService.GetProductsCountAsync(search);
        }
        var items = products.Adapt<List<GetListProductsModel>>();
        return Json(new DataTableResults<GetListProductsModel>
        {
            Data = items,
            Draw = dataTableParameters.Draw,
            RecordsTotal = totalCount,
            RecordsFiltered = filterCount
        });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveProduct(int id)
    {
        await _productService.RemoveProduct(id);
        var rows = await _applicationDbContext.SaveChangesAsync();
        return Json(new { status = rows > 0 });
    }

    public async Task<IActionResult> Pay(int id)
    {
        var factor = _applicationDbContext.Factors.FirstOrDefault(x => x.Id == id);
        if (factor.IsPaid)
        {
            return Content("Is Paid");
        }
        var price = factor.FinalPrice;
        var callback = Url.Action("Verify", "Product", null, Request.Scheme);

        var result=await _onlinePayment.RequestAsync(invoic =>
        {
            
            invoic.SetGateway("zarin1");
            invoic.SetAmount(price);
            invoic.SetCallbackUrl(callback);
            invoic.SetTrackingNumber(factor.Id);
            
            //-------
            
            invoic.SetZarinPalData(new ZarinPalInvoice("پرداخت فاکتور", "", ""));
            invoic.UseZarinPal();
        });

        if (result.IsSucceed)
        {
            //for spa or mobile
            //result.GatewayTransporter.Descriptor.Url
            await result.GatewayTransporter.TransportAsync();
        }

        return Content("Error In Gateway");
    }

    public async Task<IActionResult> Verify()
    {
        var invoice = await _onlinePayment.FetchAsync();

        if (invoice.IsAlreadyVerified)
        {
            return Content("IsAlreadyVerified");
        }

        if (invoice.IsSucceed)
        {
            //الزامی
            var result=await _onlinePayment.VerifyAsync(invoice);
            //انصراف از پرداخت
            //await _onlinePayment.CancelAsync(invoice);

            if (result.IsSucceed)
            {
                var factor = await _applicationDbContext
                    .Factors
                    .FirstOrDefaultAsync(x => x.Id == invoice.TrackingNumber);
                factor.IsPaid = true;
                await _applicationDbContext.SaveChangesAsync();

                return Content("OK");
            }
        }
             
        return Content("Failed");
    }
}