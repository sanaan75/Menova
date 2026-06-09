using Entities;
using Entities.Basics.Security;

namespace Services;

public interface IActorService
{
    void Set(Actor actor);
    Actor Get();

    bool IsAuthenticated { get; }
    bool IsSystemAdmin { get; }
    int UserId { get; }

    IList<Permission> GetPermissions();
    bool HasPermission(Permission permission);
}