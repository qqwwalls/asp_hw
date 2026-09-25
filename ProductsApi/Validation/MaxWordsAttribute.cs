using System;
using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Validation;

public class MaxWordsAttribute : ValidationAttribute
{
    private readonly int _maxWords;

    public MaxWordsAttribute(int maxWords)
    {
        _maxWords = maxWords;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        var text = value.ToString();
        if (string.IsNullOrWhiteSpace(text))
            return ValidationResult.Success;

        var wordCount = text.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
        
        if (wordCount > _maxWords)
        {
            return new ValidationResult($"Текст містить {wordCount} слів, що перевищує максимальний ліміт у {_maxWords} слів.");
        }

        return ValidationResult.Success;
    }
}
