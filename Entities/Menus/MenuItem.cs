using Entities.Users;

namespace Entities.Menus;

public class MenuItem : IEntity
{
    public int Id { get; set; }

    public User User { get; set; }
    public int UserId { get; set; }
    
    public SubCategory SubCategory { get; set; }
    public int SubCategoryId { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public int? Price { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public bool IsExist { get; set; }
    
    public ICollection<MenuItemImage> Images { get; set; }
}