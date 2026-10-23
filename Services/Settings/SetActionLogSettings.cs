using Entities;
using Entities.Settings;

namespace Services.Settings;

public class SetActionLogSettings(IDb db, ISettingsCache settingsCache) : ISetActionLogSettings
{
    public void Respond(ISetActionLogSettings.Request request)
    {
        Check.Positive(request.ActionLogMaxCount, () => MessageFactory.Positive("حداکثر تعداد گزارش‌های فعالیت"));
        Check.Positive(request.ActionLogMaxDays, () => MessageFactory.Positive("حداکثر روز نگهداری گزارش‌های فعالیت"));

        var setting = db.Set<Setting>().Single();
        setting.ActionLogMaxCount = request.ActionLogMaxCount;
        setting.ActionLogMaxDays = request.ActionLogMaxDays;

        db.AddPostSaveAction(settingsCache.Reset);
    }
}
