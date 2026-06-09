using System.Security.Cryptography;
using Entities.Users;
using Services.Settings;

namespace Services.Users;

public class AddUserConfirmCode : IAddUserConfirmCode
{
    private readonly IDb _db;
    private readonly ITimeService _timeService;
    private readonly ISettingsCache _settingsCache;

    public AddUserConfirmCode(IDb db, ITimeService timeService, ISettingsCache settingsCache)
    {
        _db = db;
        _timeService = timeService;
        _settingsCache = settingsCache;
    }

    public UserConfirmCode Respond(User user)
    {
        var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        return _db.Set<UserConfirmCode>().Add(new UserConfirmCode
        {
            User = user,
            Code = code,
            ExpireAt = _timeService.Now.AddMinutes(_settingsCache.Get().UserConfirmCodeExpire),
        }).Entity;
    }

    public UserConfirmCode Respond(int userId)
    {
        var now = _timeService.Now;
        var code = RandomNumberGenerator.GetInt32(100111, 999999).ToString();

        var activeCode = _db.Set<UserConfirmCode>().Where(i => i.UserId == userId && i.ExpireAt > now)
            .OrderByDescending(i => i.ExpireAt)
            .FirstOrDefault();

        if (activeCode is not null)
            return activeCode;

        return _db.Set<UserConfirmCode>().Add(new UserConfirmCode
        {
            UserId = userId,
            Code = code,
            ExpireAt = now.AddMinutes(_settingsCache.Get().UserConfirmCodeExpire),
        }).Entity;
    }
}