using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FoodSathi.Helpers
{
    public static class EmailHelper
    {
        private static readonly string fromEmail = "chhetrirosun@gmail.com"; // your Gmail
        private static readonly string appPassword = "knizszftctnzekwz";     // your App Password
        private static readonly string adminEmail = "admin@foodsathi.com";   // admin email address

        // ✅ Send email to any address
        public static async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromEmail, "FoodSathi Support");
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(fromEmail, appPassword);
                await smtp.SendMailAsync(mail);
            }
        }

        // ✅ Shortcut for sending admin notification
        public static async Task NotifyAdminAsync(string subject, string body)
        {
            await SendEmailAsync(adminEmail, subject, body);
        }
    }
}
