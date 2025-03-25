using System.Net.Mail;
using System.Net;

namespace FFhub_backend.Services
{
    public class MailService
    {
        public void SendMailAccessLog(string ip)
        {
            // Sender's Gmail credentials
            string senderEmail = "overflowthegame@gmail.com";
            string senderPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWD");
            string suggestionNotificationEmail = Environment.GetEnvironmentVariable("RECEIPT");

            // Create a new MailMessage
            MailMessage mail = new MailMessage(senderEmail, suggestionNotificationEmail)
            {
                Subject = "Someone accessed the website!",
                Body = $"Someone viwed ffhub from {ip}"
            };

            // Configure the SMTP client
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            try
            {
                // Send the email
                smtpClient.Send(mail);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email using password {senderPassword}: {ex.Message}");
                throw;
            }
        }
        public void SendMail()
        {
            // Sender's Gmail credentials
            string senderEmail = "overflowthegame@gmail.com";
            string senderPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWD");
            string suggestionNotificationEmail = Environment.GetEnvironmentVariable("RECEIPT");

            // Create a new MailMessage
            MailMessage mail = new MailMessage(senderEmail, suggestionNotificationEmail)
            {
                Subject = "New video recommandation on ffhub",
                Body = $"Please review"
            };

            // Configure the SMTP client
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            try
            {
                // Send the email
                smtpClient.Send(mail);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email using password {senderPassword}: {ex.Message}");
                throw;
            }
        }
    }
}
