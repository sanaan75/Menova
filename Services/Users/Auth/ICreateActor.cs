using Entities;

namespace Services.Users.Auth;

public interface ICreateActor
{
    Actor Respond(string username, string plainPassword, bool authenticated);
    Actor Respond(int userId);
}