using System.Collections.Generic;

namespace ProductsApi.DTOs;

public class OrderMessage
{
    public int UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}
