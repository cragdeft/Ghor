using System;
using System.Net;
using System.Net.Mail;
using SmartHome.Entity;
using SecurityManager = SmartHome.Utility.EncriptionAndDecryption.SecurityManager;

namespace SmartHome.MailApi.MailHelper
{
    public class SmartHomeMailClient
    {
        private const string FromPassword = "fP#DZC1A"; //(foxtrot - PAPA - Hash - DELTA - ZULU - CHARLIE - One - ALPHA)
        public EmailEntity _Email { get; set; }

        public SmartHomeMailClient(EmailEntity email)
        {
            _Email = email;
        }
        public bool SendEmail(bool isEncrypted)
        {
            
                var mailMessage = InitiateEmail(_Email, isEncrypted);

                SmtpClient smtpClient = new SmtpClient("mail.sinepulse.com");
                smtpClient.Credentials = new NetworkCredential(_Email.FromAddress, FromPassword);

                return SendMailFromSmtpClient(smtpClient, mailMessage);
        }

        public EmailEntity RetriveEncryptedEmailContent()
        {
            _Email.Body = SecurityManager.Decrypt(_Email.Body);
            return _Email;
        }

        private bool SendMailFromSmtpClient(SmtpClient smtpClient, MailMessage mailMessage)
        {
            try
            {
                smtpClient.Send(mailMessage);
                var s = mailMessage.Headers["Date"];
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private MailMessage InitiateEmail(EmailEntity email, bool isEncrypted)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(email.ToAddress);
            mailMessage.From = new MailAddress(email.FromAddress);
            mailMessage.Subject = email.Subject;
            mailMessage.Body = (isEncrypted) ? SecurityManager.Encrypt(email.Body) : email.Body;
            mailMessage.IsBodyHtml = true;
            return mailMessage;
        }
    }
}
