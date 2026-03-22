using CognitoTokenLambda.Configuration;
using CognitoTokenLambda.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace CognitoTokenLambda.Infrastructure.Email;

internal class EmailService(EmailSettings settings) : IEmailService
{
    public async Task SendTokenEmailAsync(string recipientEmail, string recipientName, string token)
    {
        if (!settings.Enabled)
            return;

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(settings.SmtpServer, settings.SmtpPort, settings.SslRequired);

            if (!string.IsNullOrWhiteSpace(settings.UserName) && !string.IsNullOrWhiteSpace(settings.Password))
                await client.AuthenticateAsync(settings.UserName, settings.Password);

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(settings.SenderName, settings.SenderAddress));
            mimeMessage.To.Add(new MailboxAddress(recipientName, recipientEmail));
            mimeMessage.Subject = "Seu token de acesso - Mecânica Hermes";
            mimeMessage.Body = new BodyBuilder
            {
                HtmlBody = BuildTokenEmailBody(recipientName, token)
            }.ToMessageBody();

            await client.SendAsync(mimeMessage);
        }
        finally
        {
            if (client.IsConnected)
                await client.DisconnectAsync(true);
        }
    }

    private static string BuildTokenEmailBody(string nome, string token)
    {
        return $"""
                <html>
                <body style="font-family: Arial, sans-serif; color: #333;">
                    <h2>Olá, {nome}!</h2>
                    <p>Segue abaixo o seu token de acesso para a <strong>Mecânica Hermes</strong>:</p>
                    <div style="background-color: #f4f4f4; padding: 15px; border-radius: 5px; margin: 20px 0; word-break: break-all;">
                        <code style="font-size: 14px;">{token}</code>
                    </div>
                    <p>Utilize este token para autenticar suas requisições à API.</p>
                    <br/>
                    <p>Atenciosamente,<br/><strong>Mecânica Hermes</strong></p>
                </body>
                </html>
                """;
    }
}
