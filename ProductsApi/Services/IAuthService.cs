using System.Threading.Tasks;
using ProductsApi.DTOs;

namespace ProductsApi.Services;

public interface IAuthService
{
    Task<string> LoginAsync(LoginDto dto);
    Task<bool> RegisterAsync(RegisterDto dto);
}
