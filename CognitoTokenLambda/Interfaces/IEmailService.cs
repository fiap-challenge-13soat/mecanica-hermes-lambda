namespace CognitoTokenLambda.Interfaces;

public interface IEmailService
{
    Task SendTokenEmailAsync(string recipientEmail, string recipientName, string token);
}
