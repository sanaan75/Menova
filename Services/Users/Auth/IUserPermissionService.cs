using Entities.Basics.Security;

namespace Services.Users.Auth;

public interface IUserPermissionService
{
    bool HasPermission(int userId, Permission permission);
    IReadOnlyCollection<Permission> GetPermissions(int userId);
}
