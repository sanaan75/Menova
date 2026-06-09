using System.ComponentModel.DataAnnotations;

namespace Entities.Validations;

public class AppLengthBetweenAttribute : ValidationAttribute
{
    private readonly int _minLength;
    private readonly int _maxLength;
    private readonly string _displayName;

    public AppLengthBetweenAttribute(string displayName, int minLength, int maxLength)
    {
        _displayName = displayName;
        _minLength = minLength;
        _maxLength = maxLength;
    }

    protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
    {
        var str = value as string;

        if (string.IsNullOrEmpty(str))
            return ValidationResult.Success;

        if (str.Length < _minLength)
        {
            var message = ValidationMessages.MinLengthMessage(_displayName, _minLength);
            return new ValidationResult(message);
        }

        if (str.Length > _maxLength)
        {
            var message = ValidationMessages.MaxLengthMessage(_displayName, _maxLength);
            return new ValidationResult(message);
        }

        return ValidationResult.Success;
    }
}