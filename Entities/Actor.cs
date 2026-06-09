using Entities.Basics.Security;
using Entities.Users;

namespace Entities;

public class Actor
{
    public bool IsAuthenticated { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Title { get; set; }
    public string Name { get; set; }
    public UserType Type { get; set; }
    public BusinessType BusinessType { get; set; }
    public List<Permission> Permissions { get; set; }

    public bool HasPermission(Permission permission)
    {
        return Type == UserType.SuperAdmin | Permissions?.Contains(permission) == true;
    }
}