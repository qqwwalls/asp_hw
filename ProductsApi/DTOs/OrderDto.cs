using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductsApi.DTOs;

public class OrderItemDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Count must be at least 1")]
    public int Count { get; set; }
}

public class OrderCreateDto
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one product is required")]
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}
