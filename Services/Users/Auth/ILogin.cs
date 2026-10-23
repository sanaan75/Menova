using Entities;

namespace Services.Users.Auth;

public interface ILogin
{
    public Actor Respond(string username, string password);
}