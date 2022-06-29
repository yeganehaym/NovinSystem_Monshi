using Monshi.Domain.Products.Entities;

namespace Monshi.Web.Models.Product
{
    public class GetListProductsModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ProductType ProductType { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
