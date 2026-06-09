namespace Entities.Basics.AdminLevels;

public class County : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Province Province { get; set; }
    public int ProvinceId { get; set; }
}