using Entities.Users;

namespace Entities.Basics.Security;

public class UserInGroup : IEntity, IUserGroupOwner
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int UserGroupId { get; set; }
    public UserGroup UserGroup { get; set; }
}