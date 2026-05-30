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
        private readonly Configurations _configurations;

        public Helpers(IOptions<SmtpSettings> smtpSettings, IOptions<Configurations> configurations)
        {
            _smtpSettings = smtpSettings.Value;
            _configurations = configurations.Value;
        }

        public void CreateLog(string error)
        {
            try
            {
                /* SAVE LOGS */
                string logpath = _configurations.LogsPath;
                string[] lines = new string[] { DateTime.Now.ToString() + " " + error + "\n\n" };
                File.AppendAllLines(logpath, lines);
            }
            catch { /* cant perform create log action */ }
        }

        public bool SendEmail(string To, string Subject, string Body)
        {
            bool success = true;

            try
            {
                MailMessage Mail = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.User, _smtpSettings.DisplayName),
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
            catch(Exception ex)
            {
                success = false;

                CreateLog("Helpers.cs - SendEmail: " + ex.Message);
            }

            return success;
        }
    }
}
