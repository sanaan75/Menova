namespace Services.Settings;

public class GetAllowedUserAccountCount(ISettingsCache settingsCache) : IGetAllowedUserAccountCount
{
    public int Respond()
    {
        settingsCache.Reset();
        return settingsCache.Get().AllowUserAccountNo;
    }
}
