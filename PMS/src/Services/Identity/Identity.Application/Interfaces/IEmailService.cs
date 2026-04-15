namespace Identity.Application.Interfaces;

public interface IEmailService
{
    Task SendConfirmationLinkAsync(string email, string link, CancellationToken cancellationToken);
}
