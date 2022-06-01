using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monshi.Data.SqlServer;

namespace WebApplication2.Components;

public class DueFactorsViewComponent:ViewComponent
{
    private ApplicationDbContext _context;

    public DueFactorsViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(int top=3)
    {
        var factors = await _context
            .Factors
            .Include(x=>x.Customer)
            .Where(x => x.DueDate.HasValue)// && x.DueDate.Value > DateTime.Now)
            .OrderBy(x => x.DueDate)
            .Skip(0)
            .Take(top)
            .ToListAsync();

        return View("default", factors);
    }
}