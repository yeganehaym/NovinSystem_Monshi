using Microsoft.EntityFrameworkCore;
using Monshi.Data.SqlServer;
using Monshi.Domain;
using Monshi.Domain.Customers;
using Monshi.Domain.Orders;
using Monshi.Domain.Products.Entities;
using Monshi.Domain.Users.Entities;

namespace Monshi.ApplicationService;

public class DatabaseInitializer:IDatabaseInitializer
{
    private ApplicationDbContext _context;

    public DatabaseInitializer(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (!await _context.Users.AnyAsync())
        {

            var user = new User()
            {
                Username = "admin",
                Password = "123456",
                FirstName = "ali",
                LastName = "rahmani",
                MobileNumber = "09365437062",
                ActivationStatus = ActivationStatus.Active,
                IsAdmin = true,
                SerialNo = "dedfgf"
            };
            
            await _context.Users.AddAsync(user);
            
            var pro1 = new Product()
            {
                Name = "LG TV",
                Price = 5000,
                Quantity = 40,
                ProductType = ProductType.Product,
                User = user
            };
            
            var pro2 = new Product()
            {
                Name = "Samsung TV",
                Price = 10000,
                Quantity = 5,
                ProductType = ProductType.Product,
                User = user
            };
            
            var pro3 = new Product()
            {
                Name = "Mic",
                Price = 50000,
                Quantity = 120,
                ProductType = ProductType.Product,
                User = user
            };
      
            _context.Products.AddRange(new []{pro1,pro2,pro3});

            var customer = new Customer()
            {
                Name = "ali",
                MobileNumber = "09365437062"
            };
            _context.Customers.Add(customer);
            var factor1 = new Factor()
            {
                Customer = customer,
                User = user,
                TotalPrice = 10000,
                Discount = 1000,
                DueDate = DateTime.Now.AddDays(3),
            };
            _context.Factors.Add(factor1);
            var order1 = new OrderItem()
            {
                Factor = factor1,
                Product = pro1,
                Quantity = 2,
                Price = pro1.Price,

            };
            var order2 = new OrderItem()
            {
                Factor = factor1,
                Product = pro3,
                Quantity = 1,
                Price = pro3.Price,

            };
            
            _context.OrderItems.AddRange(new []{order1,order2});
        }

      
       
        await _context.SaveChangesAsync();
    }
}