namespace Entities.Validations;

public static class ValidationMessages
{
    public static string MinLengthMessage(string fieldName, int min) => $"حداقل طول مجاز برای {fieldName} {min} کارکتر است.";

    public static string MaxLengthMessage(string fieldName, int max) => $"حداکثر طول مجاز برای {fieldName} {max} کارکتر است.";
    
    public static string LengthMessage(string fieldName, int length) => $"{fieldName} باید دقیقا {length} کاراکتر باشد.";
}