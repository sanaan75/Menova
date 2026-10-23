namespace Entities;

public static class DefaultMessages
{
    public static readonly string InvalidUsernameOrPassword = MessageFactory.Wrong($"{Glossary.Username} یا {Glossary.Password}");
    public static readonly string InActiveUser = $"{Glossary.User} فعال نیست.";
    public static readonly string Error500 = "خطا ناشناخته با پشتیبانی تماس بگیرید.";
    public static readonly string InvalidRequest = "درخواست نامعتبر";

    public static readonly string AlreadyRegistered = "این شماره قبلا ثبت نام شده است";

    public static string AppError() => "خطایی رخ داده است. دوباره تلاش کنید یا با پشتیبانی تماس بگیرید.";

    public static string Success_Add_Message = "{0} با موفقیت ثبت شد.";
    public static string Success_Delete_Message = "{0} با موفقیت حذف شد.";
    public static string Success_Edit_Message = "{0} با موفقیت ویرایش شد.";
}