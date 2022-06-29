using Microsoft.EntityFrameworkCore;
using Monshi.Data.SqlServer;
using Monshi.Domain.Products;
using Monshi.Domain.Products.Entities;

namespace Monshi.ApplicationService;

public class ProductService : IProductService
{
    private ApplicationDbContext _applicationDbContext;

    public ProductService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task NewProductAsync(Product product)
    {
        await _applicationDbContext.Products.AddAsync(product);
    }

    public Task<Product> FindProductAsync(int id)
    {
        throw new NotImplementedException();
    }

    private IQueryable<Product> GetQuery(string search)
    {
        var query = _applicationDbContext.Products.AsQueryable().AsNoTracking();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }
        return query;
    }

    public async Task<List<Product>> GetProductsAsync(int skip, int take, string search, int productType)
    {
        var query = GetQuery(search);
        if (productType >= 0)
        {
            var enumType = (ProductType)productType;
            query = query.Where(p => p.ProductType == enumType);
        }
        return await query.OrderByDescending(p => p.Id).Skip(skip).Take(take).ToListAsync();
    }

    public async Task<int> GetProductsCountAsync(string search)
    {
        var query = GetQuery(search);
        return await query.CountAsync();
    }

    public async Task RemoveProduct(int id)
    {
        var product = await _applicationDbContext.Products.FindAsync(id);
        if (product == null)
        {
            return;
        }
        _applicationDbContext.Products.Remove(product);
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _applicationDbContext.Products.ToListAsync();
    }
}