using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using Microsoft.Extensions.Configuration;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace AutoFull.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        MimeMessage emailService = new MimeMessage();
        emailService.From.Add(new MailboxAddress(smtpSettings["SenderName"], smtpSettings["SenderEmail"]));
        emailService.To.Add(new MailboxAddress("", email));
        emailService.Subject = subject;

        emailService.Body = new TextPart("html")
        {
            Text = $"<pre>{message}</pre>"
        };

        using SmtpClient smtp = new SmtpClient();

        try
        {
            await smtp.ConnectAsync(smtpSettings["Server"], int.Parse(smtpSettings["Port"]),
                MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpSettings["UserName"], smtpSettings["Password"]);
            await smtp.SendAsync(emailService);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}