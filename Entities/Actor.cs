using Entities.Basics.Security;
using Entities.Users;
using Services.Models;

namespace Entities;

public class Actor
{
    public bool IsAuthenticated { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Title { get; set; }
    public string Name { get; set; }
    public UserType Type { get; set; }
    public bool IsSuperAdmin { get; set; }
    public BusinessType BusinessType { get; set; }
    public List<PermissionModel> Permissions { get; init; } = [];
    public bool HasPermission(Permission permission) => IsSuperAdmin || Type == UserType.Admin || Permissions?.Any(i => ((Permission)i.Value).Includes(permission)) == true;

}