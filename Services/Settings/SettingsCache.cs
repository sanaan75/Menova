using Entities.Settings;

namespace Services.Settings;

public class SettingsCache(IContainer mainContainer) : ISettingsCache
{
    private ISettingsCache.Model _model;

    public ISettingsCache.Model Get()
    {
        if (_model is not null)
            return _model;

        using var container = mainContainer.Get<IContainer>();

        _model = container.Get<IDb>().Query<Setting>().Select(i => new ISettingsCache.Model
        {
            AllowUserAccountNo = i.AllowUserAccountNo,
            UserConfirmCodeExpire = i.UserConfirmCodeExpire,
            ResetPassExpire = i.ResetPassExpire,
            AllowedRequestsPerMinute = i.AllowedRequestsPerMinute,
            ActionLogMaxCount = i.ActionLogMaxCount,
            ActionLogMaxDays = i.ActionLogMaxDays,
            MaxFileSizeInMB = i.MaxFileSizeInMB,
            MaxLengthOfPassword = i.MaxLengthOfPassword,
            MinLengthOfPassword = i.MinLengthOfPassword,
            PasswordContainCapitalLetters = i.PasswordContainCapitalLetters,
            PasswordContainSpecialLetters = i.PasswordContainSpecialLetters,
            PasswordContainNumbers = i.PasswordContainNumbers
        }).Single();

        return _model;
    }

    public void Reset()
    {
        _model = null;
    }
}
