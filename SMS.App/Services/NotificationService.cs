using Microsoft.Extensions.Logging;
using SMS.App.Services.Interfaces;
using SMS.App.Models.Entities;
using SMS.App.Models.Config;
using MailKit.Net.Smtp;
using SMS.App.Common;
using MimeKit;

namespace SMS.App.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly IConfigurationService _configuration;

        private SmtpSettings smtpSettings;

        public NotificationService(ILogger<NotificationService> logger,
            IConfigurationService configuration)
        {
            _logger = logger;
            _configuration = configuration;

            smtpSettings = _configuration.SmtpSettings;
        }

        /// <summary>
        /// Send prepared email message using SMTP Client.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sendTo"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public async Task SendNotificationAsync(Notification notification)
        {
            var notificationMessage = CreateNewNotification(notification.Message, notification.Subject);
            using var client = CreateSmtpClient();

            try
            {
                await client.CreateConnection(_configuration);
                await client.SendAsync(notificationMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[MESSAGE-SERVICE]: Failed to send notification. [EX]: {ex}");
            }
            finally
            {
                await DisconnectFromMailServer(client);
            }

        }

        /// <summary>
        /// Creates new e-mail message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sendTo"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private MimeMessage CreateNewNotification(string content, string subject)
        {
            var newMessage = new MimeMessage();
            newMessage.From.Add(new MailboxAddress(smtpSettings.Name, smtpSettings.UserName));
            
            foreach (var item in smtpSettings.NotificationAddresses) 
            { newMessage.To.Add(MailboxAddress.Parse(item)); }

            newMessage.Subject = subject;

            newMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = content
            };

            return newMessage;
        }

        /// <summary>
        /// Returns new instance of SmtpClient.
        /// </summary>
        /// <returns></returns>
        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient();
        }

        /// <summary>
        /// Disconnect SmtpClient from the server. 
        /// </summary>
        /// <param name="smtpClient"></param>
        /// <returns></returns>
        private async Task DisconnectFromMailServer(SmtpClient smtpClient)
        {
            await smtpClient.DisconnectAsync(true);
        }
    }

    public static class MessageServiceExtensions
    {
        /// <summary>
        /// Establish authorized connection to the mail server.
        /// </summary>
        /// <param name="smtpClient"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static async Task<SmtpClient> CreateConnection(this SmtpClient smtpClient, IConfigurationService configuration)
        {
            var smtpSettings = configuration.SmtpSettings;
            var secretKey = configuration.SecretKey;

            await smtpClient.ConnectAsync(smtpSettings.Host, smtpSettings.Port, smtpSettings.EnableSsl);
            await smtpClient.AuthenticateAsync(smtpSettings.UserName, smtpSettings.Password.DecodeStringWithKey(secretKey));

            return smtpClient;
        }
    }
}
