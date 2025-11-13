using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FoodSathi.Helpers
{
    public static class EmailHelper
    {
        private static readonly string fromEmail = "teamfoodsathi@gmail.com"; 
        private static readonly string appPassword = "kcvd krny kvnb slro";    
        private static readonly string adminEmail = "admin@foodsathi.com"; 

      
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

    
        public static async Task NotifyAdminAsync(string subject, string body)
        {
            await SendEmailAsync(adminEmail, subject, body);
        }
    }
}
