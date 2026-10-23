using Entities;
using Entities.Basics.Security;
using Services.Models;

namespace Services;

public interface IActorService
{
    void Set(Actor actor);
    Actor Get();

    bool IsAuthenticated { get; }
    bool IsSuperAdmin { get; }
    bool IsAdmin { get; }
    int UserId { get; }

    IList<PermissionModel> GetPermissions();
    bool HasPermission(Permission permission);
}