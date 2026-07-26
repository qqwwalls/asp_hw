using System.ComponentModel.DataAnnotations;

namespace ProductsApi.DTOs;

public class CategoryCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
