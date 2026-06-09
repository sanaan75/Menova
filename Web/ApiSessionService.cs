using Entities;
using Services;
using Services.Users.Apis;
using Services.Users.Auth;

namespace Web;

public class ApiSessionService(IContainer mainContainer) : IApiSessionService
{
    private readonly Dictionary<string, Actor> _sessions = new();

    public Actor Login(string username, string password)
    {
        var key = $"{username}:{password}";

        if (_sessions.ContainsKey(key))
            return _sessions[key];

        using var container = mainContainer.Get<IContainer>();

        var db = container.Get<IDb>();
        var validatePassword = container.Get<IValidatePassword>();
        var createActor = container.Get<ICreateActor>();

        validatePassword.Respond(new IValidatePassword.Request
        {
            Username = username,
            Password = password
        });

        var actor = createActor.Respond(username, password, true);
        db.Save();

        _sessions.Add(key, actor);

        return actor;
    }

    public Actor Login(Guid token)
    {
        using var container = mainContainer.Get<IContainer>();

        var userApiTokenCache = container.Get<IUserApiTokenCache>();
        var model = userApiTokenCache.Get(token);

        var timeService = container.Get<ITimeService>();
        Check.Ascends(timeService.Now, model.ExpireDate, () => $"{Glossary.Token} منقضی شده‌است");

        var key = token.ToString();
        if (_sessions.ContainsKey(key))
            return _sessions[key];

        var db = container.Get<IDb>();
        var createActor = container.Get<ICreateActor>();

        var actor = createActor.Respond(model.UserId);
        db.Save();

        _sessions.Add(key, actor);

        return actor;
    }
}