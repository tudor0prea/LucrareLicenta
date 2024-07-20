using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SafeWheel3
{
    public class EmailService : IEmailSender

    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

/*            {
                "SafeWheelMailAddress": "safewheel.app@gmail.com",
  "SafeWheelMusicMailPassword": "cmuq rlis aieh pftb"
}*/
            string mailAddress = "safewheel.app@gmail.com";
            string mailPassword = "cmuq rlis aieh pftb";
            try
            {
                var fromAddress = new MailAddress(mailAddress, "Safe Wheel");
                var toAddress = new MailAddress(email);

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, mailPassword)
                };

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // TO DO: Log or handle your exception
                throw;  // Re-throwing the exception
            }
        }
    }
}
