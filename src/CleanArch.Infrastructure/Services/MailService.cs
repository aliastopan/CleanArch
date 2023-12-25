using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace CleanArch.Infrastructure.Services;

internal sealed class MailService : IMailService
{
    public async Task Send(string emailAddressTo, string emailAddressFrom, string password,
        string subject, string body, string smtpHost, int port)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(emailAddressFrom));
        email.To.Add(MailboxAddress.Parse(emailAddressTo));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html)
        {
            Text = body
        };

        using var smtp = new SmtpClient();
        smtp.Connect(smtpHost, port, SecureSocketOptions.StartTls);
        smtp.Authenticate(emailAddressFrom, password);

        await smtp.SendAsync(email);

        smtp.Disconnect(true);
    }
}
