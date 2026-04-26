using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace BudgetFlow.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;
        public EmailService(EmailSettings settings, ILogger<EmailService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string toName, string subject, string body, byte[]? attachment = null, string? attachmentName = null)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = body};

                // if there is an attachment add it to email
                if(attachment != null && attachmentName != null)
                    builder.Attachments.Add(attachmentName, attachment);

                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.UserName, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }
    }
}