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

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordDto dto,
        [FromServices] ProductsApi.Data.AppDbContext context,
        [FromServices] IEmailService emailService)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(context.Users, u => u.Email == dto.Email);
        if (user == null)
        {
            // Do not reveal that the user does not exist for security reasons
            return Ok(new { Message = "If that email address is in our database, we will send you an email to reset your password." });
        }

        var resetToken = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
        user.ResetToken = System.Convert.ToBase64String(resetToken);
        user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

        await context.SaveChangesAsync();

        await emailService.SendPasswordResetEmailAsync(user.Email, user.ResetToken);

        return Ok(new { Message = "If that email address is in our database, we will send you an email to reset your password." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordDto dto,
        [FromServices] ProductsApi.Data.AppDbContext context)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(context.Users, u => u.ResetToken == dto.Token);
        if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
        {
            return BadRequest(new { Error = "Invalid or expired token." });
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.ResetToken = null;
        user.ResetTokenExpires = null;

        await context.SaveChangesAsync();

        return Ok(new { Message = "Password has been reset successfully." });
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
