using Entities;

namespace Services.Users;

public interface ILoginApi
{
    Task<Actor> Respond(string username, string password);
}