using System.ComponentModel.DataAnnotations;

namespace ProductsApi.DTOs;

public class CreateReviewDto
{
    [Required]
    [MaxLength(100)]
    public string Author { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    [ProductsApi.Validation.MaxWords(100)]
    public string Text { get; set; } = string.Empty;

    public bool IsQuestion { get; set; }
}
