using Monshi.Domain.Products;
using Monshi.Domain.Products.Entities;

namespace Monshi.ApplicationService;

public class ProductService:IProductService
{
    public Task NewProductAsync(Product product)
    {
        throw new NotImplementedException();
    }

    public Task<Product> FindProductAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Product>> GetProductsAsync(int skip, int take, string search = null)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetProductsCountAsync(string search = null)
    {
        throw new NotImplementedException();
    }
}