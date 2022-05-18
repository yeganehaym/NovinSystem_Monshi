using Microsoft.AspNetCore.Mvc.Filters;

namespace Monshi.Domain.Logs;

public class Log:BaseEntity
{
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Paramters { get; set; }
    public string Ip { get; set; }
    public int UserId { get; set; }
    
}