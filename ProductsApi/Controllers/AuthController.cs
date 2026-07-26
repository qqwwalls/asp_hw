using Microsoft.AspNetCore.Mvc;
using ProductsApi.DTOs;
using ProductsApi.Services;
using System.Threading.Tasks;

namespace ProductsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(dto);
        if (!result)
        {
            return BadRequest(new { Error = "User with this email already exists." });
        }

        return Ok(new { Message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var token = await _authService.LoginAsync(dto);
        if (token == null)
        {
            return Unauthorized(new { Error = "Invalid email or password." });
        }

        return Ok(new { Token = token });
    }
}
