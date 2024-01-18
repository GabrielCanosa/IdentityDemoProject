using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace IdentityDemoProject.Services
{
    public class EmailSender : IEmailSender
    {
        private string SendGridKey { get; set; }
        public EmailSender(IConfiguration configuration)
        {
            SendGridKey = configuration.GetValue<string>("SendGrid:SecretKey");
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(SendGridKey);
            var from_email = new EmailAddress("hyena.games.soft@gmail.com", "Hyena Games");

            var to_email = new EmailAddress(email);

            var msg = MailHelper.CreateSingleEmail(from_email, to_email, subject, "", htmlMessage);
            await client.SendEmailAsync(msg).ConfigureAwait(false);
        }
    }
}
