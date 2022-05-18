namespace Monshi.Web.Models;

public class Occasion
{
    public string type { get; set; }
    public List<OccasionValue> values { get; set; }
}

public class OccasionValue
{
    public int Id { get; set; }
    public bool dayoff { get; set; }
    public string occasion { get; set; }
}