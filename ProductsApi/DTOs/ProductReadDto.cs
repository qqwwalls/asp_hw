using System.Collections.Generic;

namespace ProductsApi.DTOs;

public class ProductReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public List<string> ImageUrls { get; set; } = new List<string>();
}
