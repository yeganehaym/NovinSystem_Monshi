using Microsoft.AspNetCore.Mvc;
using Monshi.Data.SqlServer;
using Monshi.Domain.Products;
using Monshi.Domain.Products.Entities;
using Monshi.Domain.Users;

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
    public async Task<IActionResult> NewProduct(string name,int price)
    {
        var product = new Product()
        {
            Name = name,
            Price = price,
            ProductType = ProductType.Product
        };
        await _productService.NewProductAsync(product);
        var rows = await _applicationDbContext.SaveChangesAsync();
        if (rows > 0)
        {
            return RedirectToAction("NewProduct");
        }
        return View();
    }
}