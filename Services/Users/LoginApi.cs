using Entities;
using Services.Apis;
using Services.Users.Auth;

namespace Services.Users;

public class LoginApi(ISearchUsers searchUsers, ICreateActor createActor, IApiTokenService apiTokenService) : ILoginApi
{
    public async Task<Actor> Respond(string username, string password)
    {
        var user = searchUsers.Respond(new ISearchUsers.Request
        {
            Username = username,
        }).SingleOrDefault();

        Check.NotNull(user, () => ErrorMessagePersian.Invalid("نام کاربری یا رمز عبور"));

        var hashedPass = HashPassword.Hash(username, password);
        Check.Equal(hashedPass, user.Password, () => ErrorMessagePersian.Invalid("نام کاربری یا رمز عبور"));

        var actor = createActor.Respond(user.Username, user.Password, true);
        await apiTokenService.CreateTokenAsync(actor.UserId);

        return actor;
    }
}