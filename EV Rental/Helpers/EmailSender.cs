using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EV_Rental.Helpers
{
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default);
    }

    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;

        public SmtpEmailSender(IOptions<SmtpSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
        {
            toEmail = (toEmail ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is required", nameof(toEmail));

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSSL,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential((_settings.UserName ?? string.Empty).Trim(), (_settings.Password ?? string.Empty).Trim())
            };

            var fromEmail = string.IsNullOrWhiteSpace(_settings.FromEmail) ? _settings.UserName : _settings.FromEmail;
            var fromName = string.IsNullOrWhiteSpace(_settings.FromName) ? _settings.UserName : _settings.FromName;
            fromEmail = (fromEmail ?? string.Empty).Trim();
            fromName = (fromName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(fromEmail))
                throw new ArgumentException("SMTP FromEmail/UserName is required", nameof(_settings.FromEmail));

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            using (message)
            {
                await client.SendMailAsync(message, cancellationToken);
            }
        }
    }
}


