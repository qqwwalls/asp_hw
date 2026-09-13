using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.DTOs;
using ProductsApi.Services;

namespace ProductsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeliveryAddressController : ControllerBase
{
    private readonly IDeliveryAddressService _deliveryAddressService;

    public DeliveryAddressController(IDeliveryAddressService deliveryAddressService)
    {
        _deliveryAddressService = deliveryAddressService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(DeliveryAddressReadDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> AddAddress([FromBody] DeliveryAddressCreateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            return Unauthorized();
        }

        var created = await _deliveryAddressService.AddAddressAsync(userId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetUserAddresses), new { }, created);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DeliveryAddressReadDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetUserAddresses(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int userId))
        {
            return Unauthorized();
        }

        var addresses = await _deliveryAddressService.GetUserAddressesAsync(userId, cancellationToken);
        return Ok(addresses);
    }
}
