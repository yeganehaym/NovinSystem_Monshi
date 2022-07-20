using Monshi.Domain.Customers;
using Monshi.Domain.Users.Entities;

namespace Monshi.Domain.Orders;

public class Factor:BaseEntity
{
    public int TotalPrice { get; set; }
    public int Discount { get; set; }
    public int FinalPrice => TotalPrice - Discount;
    
    
    public User User { get; set; }
    public int? UserId { get; set; }
    
    public Customer Customer { get; set; }
    public int? CustomerId { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public List<OrderItem> OrderItems { get; set; }

    public string GetRelativeTime()
    {
        if (DueDate.HasValue == false)
            return "";

        var timeSpan = DueDate.Value - DateTime.Now;
        if ((int)timeSpan.TotalDays > 0)
            return ((int) timeSpan.TotalDays) + " روز";
        
        if((int)timeSpan.TotalHours>0)
            return ((int) timeSpan.TotalHours) + " ساعت";

  
            
        return "تا دقایق دیگر";
    }

    public bool IsPaid { get; set; }
    public bool IsPassed()
    {
        return DueDate.Value < DateTime.Now;
    }
}