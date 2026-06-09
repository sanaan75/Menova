using System.ComponentModel.DataAnnotations;

namespace Entities.Validations;

public class AppMinLengthAttribute : ValidationAttribute
{
    private readonly int _minLength;
    private readonly string _displayName;

    public AppMinLengthAttribute(string displayName, int minLength)
    {
        _displayName = displayName;
        _minLength = minLength;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var str = value as string;

        if (string.IsNullOrEmpty(str) || str.Length < _minLength)
        {
            var message = ValidationMessages.MinLengthMessage(_displayName, _minLength);
            return new ValidationResult(message);
        }

        return ValidationResult.Success;
    }
}