using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.DTOs;
using ProductsApi.Services;
using System;
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
        if (result == null)
        {
            return BadRequest(new { Error = "User with this email already exists." });
        }

        SetTokenCookie(result.RefreshToken);

        return Ok(new { Token = result.Token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(dto);
        if (result == null)
        {
            return Unauthorized(new { Error = "Invalid email or password." });
        }

        SetTokenCookie(result.RefreshToken);

        return Ok(new { Token = result.Token });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { Error = "Refresh token is missing." });
        }

        var newAccessToken = await _authService.RefreshTokenAsync(refreshToken);
        if (newAccessToken == null)
        {
            return Unauthorized(new { Error = "Refresh token is invalid or expired." });
        }

        return Ok(new { Token = newAccessToken });
    }

    private void SetTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
}
