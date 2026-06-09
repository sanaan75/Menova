using Entities;
using Entities.Basics.Security;
using Entities.Users;
using Services;
using Services.Sessions;

namespace Web;

public class ActorService(ISessionService sessionService) : IActorService
{
    private Actor _actor;

    public void Set(Actor actor)
    {
        _actor = actor;
    }

    public Actor Get()
    {
        if (_actor != null)
            return _actor;

        _actor = sessionService.GetOrStart<Actor>();

        return _actor;
    }

    public bool IsAuthenticated => Get()?.IsAuthenticated ?? false;
    public int UserId => Get().UserId;

    public bool IsSuperAdmin => Get().Type == UserType.SuperAdmin;

    public bool HasPermission(Permission permission)
    {
        return Get().HasPermission(permission);
    }

    public IList<Permission> GetPermissions()
    {
        return Get().Permissions.ToList();
    }
}