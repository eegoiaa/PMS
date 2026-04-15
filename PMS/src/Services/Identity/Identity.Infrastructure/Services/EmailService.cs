using Identity.Application.Interfaces;
using Identity.Domain.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Identity.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SmtpOptions _smtpOptions;

    public EmailService(IOptions<SmtpOptions> smtpOptions)
    {
        _smtpOptions = smtpOptions.Value;
    }

    public async Task SendConfirmationLinkAsync(string email, string link, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient(_smtpOptions.Host, _smtpOptions.Port)
        {
            Credentials = new NetworkCredential(_smtpOptions.Username, _smtpOptions.Password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpOptions.FromEmail),
            Subject = "PMS - Confirm your registration",
            Body = $"<h1>Welcome!</h1><p>Please confirm your registration by clicking <a href='{link}'>this link</a>.</p>",
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);

        await client.SendMailAsync(mailMessage, cancellationToken);
    }
}
