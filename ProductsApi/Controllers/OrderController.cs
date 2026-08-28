using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.DTOs;
using ProductsApi.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductsApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IRabbitMqService _rabbitMqService;

    public OrderController(IRabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            return Unauthorized("User ID not found in token");
        }

        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "unknown@example.com";

        var orderMessage = new OrderMessage
        {
            UserId = userId,
            UserEmail = userEmail,
            Items = dto.Items
        };

        try
        {
            await _rabbitMqService.SendMessageAsync(orderMessage, "Orders");
            return Accepted(new { message = "Your order is being processed." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to queue order.", details = ex.Message });
        }
    }
}
