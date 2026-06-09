namespace Entities.Basics.Security;

public class UserGroup : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    
    public ICollection<UserGroupPermission> Permissions { get; set; }
}