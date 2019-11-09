using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace IdentityServer.Services
{
    public class EmailSender : IEmailSender
    {

        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSsl;
        private readonly string _userName;
        private readonly string _password;
        
        public EmailSender(string host, int port, bool enableSsl, string userName, string password) {
            _host = host;
            _port = port;
            _enableSsl = enableSsl;
            _userName = userName;
            _password = password;
        }
        
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SmtpClient client = new SmtpClient(_host, _port) {
                Credentials = new NetworkCredential(_userName, _password),
                EnableSsl = _enableSsl
            };
            return client.SendMailAsync(
                new MailMessage(_userName, email, subject, htmlMessage)
                    { IsBodyHtml = true }
            );
        }

        public async Task SendEmailAsync(MimeMessage message)
        {
            message.From.Clear();
            message.From.Add(new MailboxAddress("ChatChain No-Reply", _userName));
            using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(_host, _port);

                await client.AuthenticateAsync(_userName, _password);
                
                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }
    }
}