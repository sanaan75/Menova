namespace Entities;

public static class CommonMessages
{
    public static readonly string InvalidUsernameOrPassword = ErrorMessagePersian.Wrong($"{Glossary.Username} یا {Glossary.Password}");
    public static readonly string InActiveUser = $"{Glossary.User} فعال نیست.";
    public static readonly string Error500 = "خطا ناشناخته با پشتیبانی تماس بگیرید.";
    public static readonly string InvalidRequest = "درخواست نامعتبر";

    public static readonly string AlreadyRegistered = "این شماره قبلا ثبت نام شده است";

    public static string AppError() => "خطایی رخ داده است. دوباره تلاش کنید یا با پشتیبانی تماس بگیرید.";
}