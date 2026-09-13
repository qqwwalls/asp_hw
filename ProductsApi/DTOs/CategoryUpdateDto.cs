using System.ComponentModel.DataAnnotations;

namespace ProductsApi.DTOs;

public class CategoryUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Slug { get; set; } = string.Empty;

    public int? ParentId { get; set; }
}
