using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProductsApi.Validation;

public class AllowedCountryAttribute : ValidationAttribute
{
    private readonly string[] _allowedCountries;

    public AllowedCountryAttribute(params string[] allowedCountries)
    {
        _allowedCountries = allowedCountries;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        var country = value.ToString();
        if (string.IsNullOrWhiteSpace(country))
            return ValidationResult.Success;

        if (!_allowedCountries.Contains(country, System.StringComparer.OrdinalIgnoreCase))
        {
            var allowedString = string.Join(", ", _allowedCountries);
            return new ValidationResult($"Країна '{country}' не підтримується. Дозволені країни: {allowedString}.");
        }

        return ValidationResult.Success;
    }
}
