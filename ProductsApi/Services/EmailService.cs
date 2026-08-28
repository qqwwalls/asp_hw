using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;

namespace ProductsApi.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
    {
        var emailSettings = _config.GetSection("EmailSettings");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Password Reset Request";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $"<p>You requested a password reset. Your reset token is:</p><h3>{resetToken}</h3><p>Please use this token in the reset password API endpoint to set a new password.</p>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            using var client = new SmtpClient();
            // Using a dummy SMTP server or sandbox for homework. It might fail if credentials are fake,
            // so we wrap in try-catch and log to console to not crash the app.
            await client.ConnectAsync(emailSettings["SmtpHost"], int.Parse(emailSettings["SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(emailSettings["SmtpUsername"], emailSettings["SmtpPassword"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            System.Console.WriteLine($"Email sent to {toEmail} with token: {resetToken}");
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"Failed to send email. Ensure SMTP settings in appsettings.json are valid. The token is: {resetToken}. Error: {ex.Message}");
        }
    }
}
