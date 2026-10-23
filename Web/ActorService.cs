using System.Text.Json;
using Entities;
using Entities.Users;
using Services;
using Services.Sessions;

namespace Web;

public class ActorService(ISessionService sessionService) : IActorService
{
    private Actor _actor;

    public void Set(Actor actor)
    {
        _actor = NormalizeLegacyType(actor);
    }

    public Actor Get()
    {
        if (_actor != null)
            return NormalizeLegacyType(_actor);

        try
        {
            _actor = sessionService.GetOrStart<Actor>();
        }
        catch (JsonException)
        {
            // Start a clean anonymous actor when an older session cannot be read.
            _actor = sessionService.Start(new Actor(), authenticated: false, username: null);
        }

        return NormalizeLegacyType(_actor);
    }

    public bool IsAuthenticated => Get()?.IsAuthenticated ?? false;
    public int UserId => Get().UserId;

    public bool IsAdmin => Get().Type == UserType.Admin;

    private static Actor NormalizeLegacyType(Actor actor)
    {
        if ((int)actor.Type == 32)
        {
            actor.Type = UserType.Admin;
        }
        else if (actor.Type is not (UserType.Customer or UserType.Admin))
        {
            actor.IsAuthenticated = false;
            actor.UserId = 0;
        }

        return actor;
    }
}
