using Entities.Users;

namespace Entities.Menus;

public class Menu : IEntity
{
    public int Id { get; set; }

    public User User { get; set; }
    public int UserId { get; set; }
    
    public string Name { get; set; }
    public string Description { get; set; }
    
    public ICollection<Category> Categories { get; set; }
}