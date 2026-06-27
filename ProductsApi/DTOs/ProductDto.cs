using System.ComponentModel.DataAnnotations;

namespace ProductsApi.DTOs;

public class ProductDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
}
