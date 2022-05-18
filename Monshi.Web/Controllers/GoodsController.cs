using System.Net;
using Microsoft.AspNetCore.Mvc;
using Monshi.Data.SqlServer;
using Monshi.Domain.Products.Entities;
using Monshi.Web.Models;

namespace Monshi.Web.Controllers;

[Route("/api/[controller]/[action]/{id?}")]
[ApiController]
public class GoodsController:ControllerBase
{
    private ApplicationDbContext _context;

    public GoodsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public List<ProductItem> Get()
    {
        var products = _context.Products.ToList();
        var items = products
            .Select(x => new ProductItem()
            {
                Name=x.Name,
                Price = x.Price
            })
            .ToList();

        return items;
    }
    
    [HttpGet]
    public ProductItem GetOne(int id)
    {
        var product = _context.Products.Find(id);
        var item = new ProductItem()
        {
            Name = product.Name,
            Price = product.Price
        };

        return item;
    }

    [HttpPost]
    public IActionResult Post([FromBody]ProductItem item)
    {
        var product = new Product()
        {
            Name = item.Name,
            Price = item.Price,
            ProductType = ProductType.Product,
            CreationDate = DateTime.Now
        };

        _context.Products.Add(product);
        var rows = _context.SaveChanges();
        if (rows > 0)
            return Created("Post",new {id=product.Id});
        return BadRequest();
    }
    
    [HttpPost]
    public IActionResult Update([FromBody]ProductItem item)
    {
        var product = _context.Products.Find(item.Id);
        product.Name = item.Name;
        product.Price = item.Price;
        var rows = _context.SaveChanges();
        if (rows > 0)
            return Ok();
        return BadRequest();
    }
}