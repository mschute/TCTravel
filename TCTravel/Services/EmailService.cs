using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using TCTravel.Service;

namespace TCTravel.Services;


public class EmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public void SendEmail(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Tay Country Travel", _emailSettings.SmtpUsername));
        message.To.Add(new MailboxAddress("Beloved client", toEmail));
        message.Subject = subject;
        var textPart = new TextPart("plain") { Text = body };
        message.Body = textPart;
        using (var client = new SmtpClient())
        {
            client.Connect(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            client.Authenticate(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}