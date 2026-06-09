using System.ComponentModel.DataAnnotations;

namespace Entities.Validations;

public class AppMaxLengthAttribute : ValidationAttribute
{
    private readonly int _maxLength;
    private readonly string _displayName;

    public AppMaxLengthAttribute(string displayName, int maxLength)
    {
        _displayName = displayName;
        _maxLength = maxLength;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var str = value as string;

        if (!string.IsNullOrEmpty(str) && str.Length > _maxLength)
        {
            var message = ValidationMessages.MaxLengthMessage(_displayName, _maxLength);
            return new ValidationResult(message);
        }

        return ValidationResult.Success;
    }
}