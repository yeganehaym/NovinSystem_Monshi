using Microsoft.EntityFrameworkCore;
using Monshi.Data.SqlServer;
using Monshi.Domain;
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
            await _context.Users.AddAsync(new User()
            {
                Username = "admin",
                Password = "123456",
                FirstName = "ali",
                LastName = "rahmani",
                MobileNumber = "09365437062",
                ActivationStatus = ActivationStatus.Active,
                IsAdmin = true,
                SerialNo = "dedfgf"
            });
        }

       
        await _context.SaveChangesAsync();
    }
}