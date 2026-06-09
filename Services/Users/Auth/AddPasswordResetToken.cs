using Entities.Users;
using Services.Security;
using Services.Settings;

namespace Services.Users.Auth;

public class AddPasswordResetToken(
    IDb db,
    IRandomService randomService,
    ITimeService timeService,
    ISettingsCache settingsCache)
    : IAddPasswordResetToken
{
    public PasswordResetToken Respond(IAddPasswordResetToken.Request request)
    {
        var token = randomService.Next(12111, 99999).ToString();
        
        return db.Set<PasswordResetToken>().Add(new PasswordResetToken
        {
            UserId = request.UserId,
            Token = token,
            ExpireDate = timeService.Now.AddMinutes(settingsCache.Get().ResetPassExpire),
            Used = false
        }).Entity;
    }
}