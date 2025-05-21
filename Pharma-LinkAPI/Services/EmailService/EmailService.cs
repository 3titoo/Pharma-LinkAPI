
using System.Net;
using System.Net.Mail;

namespace Pharma_LinkAPI.Services.EmailService
{
    public class EmailService : IEmailService
    {

        public async Task SendEmailAsync(string recipter, string subject, string body)
        {
            string email = "pharmalink38@gmail.com";
            string password = "jfkw oedt ahxc qgoy";
            string host = "smtp.gmail.com";
            int port = 587;
            var smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(email, password),
                EnableSsl = true,
                UseDefaultCredentials = false
            };
            var message = new MailMessage(email, recipter, subject, body);
            await smtpClient.SendMailAsync(message);

        }
    }
}
