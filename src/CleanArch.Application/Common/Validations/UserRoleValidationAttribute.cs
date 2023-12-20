#nullable disable
using System.ComponentModel.DataAnnotations;
using CleanArch.Domain.Enums;

namespace CleanArch.Application.Common.Validations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class UserRoleValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        string stringValue = value.ToString();
        if(Enum.TryParse(typeof(UserRole), stringValue, out _))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult($"The field {validationContext.DisplayName} must be a valid UserRole enum.");
    }
}
