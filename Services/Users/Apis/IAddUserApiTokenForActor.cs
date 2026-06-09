using Entities.Users;

namespace Services.Users.Apis;

public interface IAddUserApiTokenForActor
{
    UserApiToken Respond(string note, bool? longLived = false);
}