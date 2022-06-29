using Mapster;
using Microsoft.AspNetCore.Mvc;
using Monshi.Data.SqlServer;
using Monshi.Domain.Products;
using Monshi.Domain.Products.Entities;
using Monshi.Domain.Users;
using Monshi.Web.Models.DataTables;
using Monshi.Web.Models.Product;

namespace Monshi.Web.Controllers;

public class ProductController : Controller
{
    private ApplicationDbContext _applicationDbContext;
    private IUserService _userService;
    private IProductService _productService;

    public ProductController(ApplicationDbContext applicationDbContext, IUserService userService, IProductService productService)
    {
        _applicationDbContext = applicationDbContext;
        _userService = userService;
        _productService = productService;
    }

    [HttpGet]
    public IActionResult NewProduct()
    {
        return View();
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
            return RedirectToAction("NewProduct");
        }
        return View();
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
}