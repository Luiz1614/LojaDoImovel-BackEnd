using LojaDoImovel.Application.Services.Interfaces;
using LojaDoImovel.Application.Settings;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Text.RegularExpressions;

public class EmailService : IEmailService
{
    private static readonly Regex EmailRegex = new(
        @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(List<string> emailsTo, string subject, string body, List<string>? attachments = null)
    {
        ArgumentNullException.ThrowIfNull(emailsTo);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);

        var validEmails = emailsTo.Where(ValidateEmail).ToList();

        if (validEmails.Count == 0)
            throw new ArgumentException("Nenhum e-mail válido foi informado.", nameof(emailsTo));

        var message = PrepareMessage(validEmails, subject, body, attachments ?? []);

        try
        {
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("E-mail enviado para: {Recipients}", string.Join(", ", validEmails));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar e-mail para: {Recipients}", string.Join(", ", validEmails));
            throw;
        }
    }

    private MimeMessage PrepareMessage(List<string> emailsTo, string subject, string body, List<string> attachments)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("Loja do Imóvel", _settings.Username));

        foreach (var email in emailsTo)
            message.To.Add(MailboxAddress.Parse(email));

        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body,
            TextBody = Regex.Replace(body, "<.*?>", string.Empty)
        };

        foreach (var file in attachments)
        {
            if (!File.Exists(file))
            {
                _logger.LogWarning("Anexo não encontrado, ignorado: {File}", file);
                continue;
            }
            bodyBuilder.Attachments.Add(file);
        }

        message.Body = bodyBuilder.ToMessageBody();
        return message;
    }

    private static bool ValidateEmail(string email) =>
        !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
}