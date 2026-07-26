using System.ComponentModel.DataAnnotations;

namespace ProductsApi.DTOs;

public class CategoryUpdateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
