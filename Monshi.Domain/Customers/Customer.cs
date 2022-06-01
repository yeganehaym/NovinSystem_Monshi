using Monshi.Domain.Orders;
using Monshi.Domain.Users.Entities;

namespace Monshi.Domain.Customers;

public class Customer:BaseEntity
{
    public User User { get; set; }
    public int? UserId { get; set; }
    
    public string Name { get; set; }
    public string MobileNumber { get; set; }
    
    public List<Factor> Factors { get; set; }
}