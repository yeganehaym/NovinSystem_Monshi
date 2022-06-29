using Monshi.Domain.Products.Entities;

namespace Monshi.Domain.Products;

public interface IProductService
{
    Task NewProductAsync(Product product);
    Task<Product> FindProductAsync(int id);
    Task<List<Product>> GetProductsAsync(int skip, int take, string search, int productType);
    Task<int> GetProductsCountAsync(string search = null);
    Task RemoveProduct(int productId);
    Task<List<Product>> GetAllProductsAsync();
}