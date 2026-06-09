namespace Entities.Basics.AdminLevels;

public class Province : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public ICollection<County> Counties { get; set; }
}