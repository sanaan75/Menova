using System.ComponentModel.DataAnnotations;

namespace Entities.Validations;

public sealed class AppRequiredAttribute : ValidationAttribute
{
    public AppRequiredAttribute()
    {
        ErrorMessage = CommonMessages.InvalidRequest;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // null
        if (value == null)
            return new ValidationResult(ErrorMessage);

        if (value is string str && string.IsNullOrWhiteSpace(str))
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}