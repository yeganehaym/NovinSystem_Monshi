namespace Monshi.Domain;

public class BaseEntity
{
    /*public BaseEntity()
    {
        CreationDate = DateTime.Now;
    }*/
    public int Id { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    
    public bool IsRemoved { get; set; }
}