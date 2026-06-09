namespace Entities.Menus;

public class Category : IEntity
{
    public int Id { get; set; }

    public Menu Menu { get; set; }
    public int MenuId { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public int Order { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }

    public ICollection<SubCategory> SubCategories { get; set; }
    public ICollection<CategoryNotify> Notifies { get; set; }
}