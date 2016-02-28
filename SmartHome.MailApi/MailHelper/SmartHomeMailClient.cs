using System;
using System.Net;
using System.Net.Mail;
using SmartHome.Entity;
using SecurityManager = SmartHome.Utility.EncryptionAndDecryption.SecurityManager;

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
        public bool SendEmail()
        {
                var mailMessage = InitiateEmail(_Email);

                SmtpClient smtpClient = new SmtpClient("mail.sinepulse.com");
                return SendMailFromSmtpClient(smtpClient, mailMessage);
        }

        private bool SendMailFromSmtpClient(SmtpClient smtpClient, MailMessage mailMessage)
        {
            try
            {
                smtpClient.Credentials = new NetworkCredential(_Email.From, FromPassword);
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private MailMessage InitiateEmail(EmailEntity email)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(email.To);
            mailMessage.From = new MailAddress(email.From);
            mailMessage.Subject = email.Subject;
            mailMessage.Body = email.Body;
            mailMessage.IsBodyHtml = true;

            return mailMessage;
        }
    }
}
