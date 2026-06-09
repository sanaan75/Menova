using System.ComponentModel.DataAnnotations;

namespace Entities.Validations;

public class AppLengthEqualsAttribute : ValidationAttribute
{
    private readonly string _displayName;
    private readonly int _length;

    public AppLengthEqualsAttribute(string displayName, int length)
    {
        _displayName = displayName;
        _length = length;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        var strValue = value.ToString();

        if (strValue.Length != _length)
        {
            var errorMessage = ValidationMessages.LengthMessage(_displayName, _length);
            return new ValidationResult(errorMessage);
        }

        return ValidationResult.Success;
    }
}