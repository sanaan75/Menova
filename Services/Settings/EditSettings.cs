using Entities;
using Entities.Settings;

namespace Services.Settings;

public class EditSettings(IDb db, ISettingsCache settingsCache) : IEditSettings
{
    public void Respond(IEditSettings.Request request)
    {
        Check.Positive(request.AllowUserAccountNo, () => MessageFactory.Positive("تعداد دستگاه مجاز برای ورود"));
        Check.Positive(request.UserConfirmCodeExpire, () => MessageFactory.Positive("مدت اعتبار کد تأیید کاربر"));
        Check.Positive(request.ResetPassExpire, () => MessageFactory.Positive("مدت اعتبار کد بازیابی رمز عبور"));
        Check.Positive(request.AllowedRequestsPerMinute, () => MessageFactory.Positive("تعداد درخواست مجاز در دقیقه"));
        Check.Positive(request.ActionLogMaxCount, () => MessageFactory.Positive("حداکثر تعداد گزارش‌های فعالیت"));
        Check.Positive(request.ActionLogMaxDays, () => MessageFactory.Positive("حداکثر روز نگهداری گزارش‌های فعالیت"));
        Check.Positive(request.MaxFileSizeInMB, () => MessageFactory.Positive("حداکثر حجم فایل به مگابایت"));
        Check.Positive(request.MaxLengthOfPassword, () => MessageFactory.Positive("حداکثر طول رمز عبور"));
        Check.Positive(request.MinLengthOfPassword, () => MessageFactory.Positive("حداقل طول رمز عبور"));
        Check.True(request.MinLengthOfPassword <= request.MaxLengthOfPassword, () => "حداقل طول رمز عبور نباید از حداکثر طول آن بیشتر باشد.");

        var setting = db.Set<Setting>().SingleOrDefault();
        Check.NotNull(setting, () => MessageFactory.NotFound(Glossary.Settings));

        setting.AllowUserAccountNo = request.AllowUserAccountNo;
        setting.UserConfirmCodeExpire = request.UserConfirmCodeExpire;
        setting.ResetPassExpire = request.ResetPassExpire;
        setting.AllowedRequestsPerMinute = request.AllowedRequestsPerMinute;
        setting.ActionLogMaxCount = request.ActionLogMaxCount;
        setting.ActionLogMaxDays = request.ActionLogMaxDays;
        setting.MaxFileSizeInMB = request.MaxFileSizeInMB;
        setting.MaxLengthOfPassword = request.MaxLengthOfPassword;
        setting.MinLengthOfPassword = request.MinLengthOfPassword;
        setting.PasswordContainCapitalLetters = request.PasswordContainCapitalLetters;
        setting.PasswordContainSpecialLetters = request.PasswordContainSpecialLetters;
        setting.PasswordContainNumbers = request.PasswordContainNumbers;

        db.AddPostSaveAction(settingsCache.Reset);
    }
}
