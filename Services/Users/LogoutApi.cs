using Services.Apis;

namespace Services.Users;

public class LogoutApi(IApiTokenService apiTokenService) : ILogoutApi
{
    public void Respond(int userId)
    {
        apiTokenService.RevokeAllUserTokensAsync(userId);
    }
}