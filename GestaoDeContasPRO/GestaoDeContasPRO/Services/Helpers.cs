using System.Net;
using System.Net.Mail;
using System.Text;
using GestaoDeContasPRO.Models;
using Microsoft.Extensions.Options;

namespace GestaoDeContasPRO.Services
{
    public class Helpers
    {
        private readonly SmtpSettings _smtpSettings;

        public Helpers(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public void SendEmail(string To, string Subject, string Body)
        {
            try
            {
                MailMessage Mail = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.User, "Gestão de contas"),
                    Subject = Subject,
                    Body = Body,
                    IsBodyHtml = true
                };
                Mail.To.Add(To);

                var password = Encoding.UTF8.GetString(Convert.FromBase64String(_smtpSettings.Password));

                SmtpClient Smtp = new SmtpClient(_smtpSettings.SmtpServer)
                {
                    Port = _smtpSettings.SmtpPort,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_smtpSettings.User, password),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                Smtp.Send(Mail);
            }
            catch { }
        }

        public void CreateLog()
        {

        }
    }
}
