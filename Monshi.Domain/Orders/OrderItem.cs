using Monshi.Domain.Products.Entities;

namespace Monshi.Domain.Orders;

public class OrderItem:BaseEntity
{
    public Product Product { get; set; }
    public int? ProductId { get; set; }
    
    public int Quantity { get; set; }
    
    public int Price { get; set; }

    public int FinalPrice => Price * Quantity - Discount;
    
    public int Discount { get; set; }

    public Factor Factor { get; set; }
    public int? FactorId { get; set; }
}