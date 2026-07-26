using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductsApi.DTOs;

public class ProductCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public List<IFormFile> Images { get; set; } = new List<IFormFile>();
}
