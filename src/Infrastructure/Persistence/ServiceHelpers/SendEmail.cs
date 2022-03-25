
using Hangfire;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Infrastructure.Persistence.ServiceHelpers.SendMailService
{
    public interface ISendEmail
    {
        Task SendEmailAsync(string? toEmail, string? subject, string body);

    }

    public class SendEmail : ISendEmail
    {
        private readonly IConfiguration Configuration;

        public SendEmail(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task SendEmailAsync(string? toEmail, string? subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(Configuration["MailSettings:Mail"]));

            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            try
            {
                smtp.Connect(Configuration!["MailSettings:Host"], int.Parse(Configuration!["MailSettings:Port"]), SecureSocketOptions.Auto);
                smtp.Authenticate(Configuration!["MailSettings:Mail"], Configuration!["MailSettings:Password"]);
                await smtp.SendAsync(email);
            }
            catch (System.Exception ex)
            {
                BackgroundJob.Enqueue(() => Console.WriteLine($"--> send mail failed: {ex.Message}"));
            }
            finally
            {
                smtp.Disconnect(true);
            }
        }
    }
}