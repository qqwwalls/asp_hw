using System.ComponentModel.DataAnnotations;

namespace ProductsApi.DTOs;

public class CategoryCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}
