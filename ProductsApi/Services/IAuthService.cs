using System.Threading.Tasks;
using ProductsApi.DTOs;

namespace ProductsApi.Services;

public interface IAuthService
{
    Task<AuthResultDto?> LoginAsync(LoginDto dto);
    Task<AuthResultDto?> RegisterAsync(RegisterDto dto);
    Task<string?> RefreshTokenAsync(string refreshToken);
    Task<AuthResultDto?> ExternalLoginAsync(string email, string providerName, string providerId);
}
