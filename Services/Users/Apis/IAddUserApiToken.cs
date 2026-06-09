using Entities.Users;

namespace Services.Users.Apis;

public interface IAddUserApiToken
{
    UserApiToken Respond(int userId, DateTime expireDate, string note);
}