namespace Entities.Menus;

public class SubCategory : IEntity
{
    public int Id { get; set; }

    public Category Category { get; set; }
    public int CategoryId { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public int Order { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }

    public ICollection<MenuItem> Items { get; set; }
    public ICollection<SubCategoryNotify> Notifies { get; set; }
}