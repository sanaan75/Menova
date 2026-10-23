using Entities.Users;
using Services.Logs;

namespace Services.Users.Logs;

public class AddUserLog(IAddLog addLog) : IAddUserLog
{
    public void Respond(User user, string note)
    {
        addLog.Respond(new UserLog
        {
            User = user
        }, note);
    }

    public void RespondAsSelf(User user, string note)
    {
        addLog.Respond(new UserLog
        {
            User = user,
            Doer = user
        }, note);
    }

    public void Respond(int userId, string note)
    {
        addLog.Respond(new UserLog
        {
            UserId = userId
        }, note);
    }
}
