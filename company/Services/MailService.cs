using System.Threading.Tasks;

using company.Models.Settings;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;


using System;
using System.Collections.Generic;
using System.Linq;



namespace company.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings Settings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            Settings = mailSettings.Value;
        }
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(Settings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEMail));
            email.Subject = mailRequest.Subject;

            email.Body = new TextPart("html") { Text = mailRequest.Body };
            
            using var smtp = new SmtpClient();
            smtp.Connect(Settings.Host, Settings.Port, SecureSocketOptions.StartTlsWhenAvailable);
            smtp.Authenticate(Settings.Mail, Settings.Password);
            await smtp.SendAsync(email); 
            smtp.Disconnect(true);

            
        }
    }
}
