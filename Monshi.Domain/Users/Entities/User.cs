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
}