namespace LojaDoImovel.Application.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(List<string> emailsTo, string subject, string body, List<string>? attachments = null);
}