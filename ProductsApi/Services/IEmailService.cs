using System.Threading.Tasks;

namespace ProductsApi.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetToken);
    Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
}
