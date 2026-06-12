namespace Entities.Menus;

public class MenuItemProperty : IEntity
{
    public int Id { get; set; }
    
    public MenuItem MenuItem { get; set; }
    public int MenuItemId { get; set; }
    
    public string Title { get; set; }
    public int? Price { get; set; }
}