namespace Monshi.Domain.Products.Entities;

public class Product:BaseEntity
{
    public string Name { get; set; }
    public int Price { get; set; }
    public ProductType ProductType { get; set; }
    //public int Quantity { get; set; }
    
   
}

public enum ProductType
{
    Product,
    Service
}