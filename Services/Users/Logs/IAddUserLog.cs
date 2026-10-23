using Entities.Users;

namespace Services.Users.Logs;

public interface IAddUserLog
{
    void Respond(User user, string note);
    void RespondAsSelf(User user, string note);
    void Respond(int userId, string note);
}
