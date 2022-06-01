using Monshi.Domain.Customers;
using Monshi.Domain.Orders;
using Monshi.Domain.Products.Entities;

namespace Monshi.Domain.Users.Entities;

public class User:BaseEntity
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string SerialNo { get; set; }
    public string MobileNumber { get; set; }
    public bool IsAdmin { get; set; }
    public  ActivationStatus ActivationStatus { get; set; }
    
    public TimeSpan ModateHozoor { get; set; }
    
    public List<Customer> Customers { get; set; }
    public List<Factor> Factors { get; set; }
    public List<Product> Products { get; set; }
}