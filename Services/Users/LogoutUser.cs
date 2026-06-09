using Entities;

namespace Services.Users;

public class LogoutUser : ILogoutUser
{
    private readonly IContainer _mainContainer;

    public LogoutUser(IContainer mainContainer)
    {
        _mainContainer = mainContainer;
    }

    public void Respond(string username)
    {
        using var container = _mainContainer.Get<IContainer>();
        var db = container.Get<IStatisticDb>();
        var timeService = container.Get<ITimeService>();
        var sessions = db.Set<AppSession>().Where(s => s.Username == username).ToList();

        foreach (var item in sessions)
        {
            item.LastAccess = timeService.Now;
            item.Authenticated = false;
        }

        db.Save();
    }
}