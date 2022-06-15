using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Monshi.Data.SqlServer;
using Monshi.Domain.Products.Entities;
using Monshi.Domain.Users.Entities;
using Monshi.Web.Models;
using WebApplication2;

namespace Monshi.Web.Controllers;

[Route("/api/[controller]/[action]/{id?}")]
[ApiController]
public class GoodsController:ControllerBase
{
    private ApplicationDbContext _context;
    private IConfiguration _configuration;
    private IStringLocalizer<GoodsController> _stringLocalizer;

    public GoodsController(ApplicationDbContext context, IConfiguration configuration, IStringLocalizer<GoodsController> stringLocalizer)
    {
        _context = context;
        _configuration = configuration;
        _stringLocalizer = stringLocalizer;
    }

    [HttpPost]
    public IActionResult Login([FromBody]LoginViewModel model)
    {
        var user = _context
            .Users
            .FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password.Hash());
        if (user == null)
            return NoContent();
        var token = GenerateJSONWebToken(user);

        return Ok(new {token});

    }

    private string GenerateJSONWebToken(User userInfo)    
    {    
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));    
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);    
    
        var claims = new[] {    
            new Claim(JwtRegisteredClaimNames.Name, userInfo.Username),    
            new Claim(JwtRegisteredClaimNames.NameId, userInfo.Id.ToString())    ,
            new Claim("SerialNo", userInfo.SerialNo)   , 
            new Claim("Role", "Hesabdar")    ,
            new Claim("Role", "Anbardar")    
        };   
        
        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],    
            _configuration["Jwt:Issuer"],    
            claims,    
            expires: DateTime.Now.AddMinutes(120),    
            signingCredentials: credentials);    
    
        return new JwtSecurityTokenHandler().WriteToken(token);    
    }  
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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