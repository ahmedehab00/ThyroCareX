using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Service.Impelemanation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            try
            {
                var host = _configuration["EmailSettings:Host"];
                var portStr = _configuration["EmailSettings:Port"];
                var email = _configuration["EmailSettings:Email"];
                var password = _configuration["EmailSettings:Password"];

                // Fallback / Debug log if real SMTP is not setup
                if (string.IsNullOrEmpty(host) || password == "dummy-app-password")
                {
                    _logger.LogWarning($"[MOCK EMAIL SEND] To: {toEmail}, Subject: {subject}");
                    _logger.LogWarning($"[MOCK EMAIL BODY]: {htmlMessage}");
                    return true;
                }

                int port = int.TryParse(portStr, out int p) ? p : 587;

                using (var client = new SmtpClient(host, port))
                {
                    client.Credentials = new NetworkCredential(email, password);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(email, "ThyraX Team"),
                        Subject = subject,
                        Body = htmlMessage,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {toEmail}. Error: {ex.Message}");
                return false;
            }
        }
    }
}
