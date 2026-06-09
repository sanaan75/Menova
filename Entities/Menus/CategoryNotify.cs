namespace Entities.Menus;

public class CategoryNotify : IEntity
{
    public int Id { get; set; }

    public Category Category { get; set; }
    public int CategoryId { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public int Order { get; set; }
    public bool IsActive { get; set; }
}