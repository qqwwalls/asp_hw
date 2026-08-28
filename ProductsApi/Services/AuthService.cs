using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProductsApi.Data;
using ProductsApi.DTOs;
using ProductsApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ProductsApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResultDto?> RegisterAsync(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return null;
        }

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(_config.GetValue<int>("JwtSettings:RefreshTokenExpiryDays")),
            Created = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return new AuthResultDto
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResultDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return null;
        }

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(_config.GetValue<int>("JwtSettings:RefreshTokenExpiryDays")),
            Created = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return new AuthResultDto
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<string?> RefreshTokenAsync(string refreshToken)
    {
        var rt = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (rt == null || rt.Expires < DateTime.UtcNow)
        {
            return null;
        }

        return GenerateJwtToken(rt.User);
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["JwtSettings:SecretKey"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("JwtSettings:ExpiryMinutes")),
            Issuer = _config["JwtSettings:Issuer"],
            Audience = _config["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
