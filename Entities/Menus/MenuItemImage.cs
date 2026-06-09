namespace Entities.Menus;

public class MenuItemImage : IEntity
{
    public int Id { get; set; }

    public MenuItem MenuItem { get; set; }
    public int MenuItemId { get; set; }
    
    public string ImageUrl { get; set; }
}