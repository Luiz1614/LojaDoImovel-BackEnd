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
        var invalidEmails = emailsTo.Except(validEmails).ToList();

        if (invalidEmails.Count > 0)
            _logger.LogWarning("E-mails inválidos ignorados: {InvalidEmails}", string.Join(", ", invalidEmails));

        if (validEmails.Count == 0)
        {
            _logger.LogError("Nenhum e-mail válido informado para o assunto '{Subject}'.", subject);
            throw new ArgumentException("Nenhum e-mail válido foi informado.", nameof(emailsTo));
        }

        _logger.LogInformation("Preparando envio de e-mail. Assunto: '{Subject}'. Destinatários: {Recipients}.", subject, string.Join(", ", validEmails));

        var message = PrepareMessage(validEmails, subject, body, attachments ?? []);

        try
        {
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            _logger.LogInformation("Conexão SMTP estabelecida com {Host}:{Port}.", _settings.Host, _settings.Port);

            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            _logger.LogInformation("Autenticação SMTP bem-sucedida.");

            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("E-mail enviado com sucesso para: {Recipients}. Assunto: '{Subject}'.", string.Join(", ", validEmails), subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar e-mail para: {Recipients}. Assunto: '{Subject}'.", string.Join(", ", validEmails), subject);
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