using System.Text.Json;
using Entities;
using Entities.Basics.Security;
using Entities.Users;
using Services;
using Services.Models;
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

        try
        {
            _actor = sessionService.GetOrStart<Actor>();
        }
        catch (JsonException)
        {
            // Older sessions may contain permission codes or dictionaries instead of the current actor lists.
            _actor = sessionService.Start(new Actor(), authenticated: false, username: null);
        }

        return _actor;
    }

    public bool IsAuthenticated => Get()?.IsAuthenticated ?? false;
    public int UserId => Get().UserId;

    public bool IsAdmin => Get().Type == UserType.Admin;
    public bool IsSuperAdmin => Get().IsSuperAdmin;

    public bool HasPermission(Permission permission) => Get().HasPermission(permission);

    public IList<PermissionModel> GetPermissions() => Get().Permissions?.ToList() ?? [];
}
