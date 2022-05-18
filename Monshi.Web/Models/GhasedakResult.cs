namespace Monshi.Web.Models;

public class GhasedakResult
{
    public Result result { get; set; }
    public List<long> items { get; set; }
}

public class Result
{
    public int code { get; set; }
    public string message { get; set; }
}