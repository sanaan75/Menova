namespace Entities.Menus;

public class SubCategoryNotify : IEntity
{
    public int Id { get; set; }

    public SubCategory SubCategory { get; set; }
    public int SubCategoryId { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public int Order { get; set; }
    public bool IsActive { get; set; }
}