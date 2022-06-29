namespace Monshi.Web.Models.DataTables;

public class DataTableResults<T> where T:class
{
    public List<T> Data { get; set; }
    public int Draw { get; set; }
    public int RecordsTotal { get; set; }
    public int RecordsFiltered { get; set; }
}