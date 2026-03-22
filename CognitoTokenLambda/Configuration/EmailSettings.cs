namespace CognitoTokenLambda.Configuration;

public class EmailSettings
{
    public bool Enabled { get; init; }
    public string SmtpServer { get; init; } = default!;
    public int SmtpPort { get; init; }
    public bool SslRequired { get; init; }
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public string SenderName { get; init; } = default!;
    public string SenderAddress { get; init; } = default!;

    public static EmailSettings FromEnvironment() => new()
    {
        Enabled = bool.TryParse(Environment.GetEnvironmentVariable("EmailSender__Enabled"), out var enabled) && enabled,
        SmtpServer = Environment.GetEnvironmentVariable("EmailSender__SmtpServer") ?? string.Empty,
        SmtpPort = int.TryParse(Environment.GetEnvironmentVariable("EmailSender__SmtpPort"), out var port) ? port : 25,
        SslRequired = bool.TryParse(Environment.GetEnvironmentVariable("EmailSender__SslRequired"), out var ssl) && ssl,
        UserName = Environment.GetEnvironmentVariable("EmailSender__UserName"),
        Password = Environment.GetEnvironmentVariable("EmailSender__Password"),
        SenderName = Environment.GetEnvironmentVariable("EmailSender__SenderInformation__Name") ?? string.Empty,
        SenderAddress = Environment.GetEnvironmentVariable("EmailSender__SenderInformation__Address") ?? string.Empty
    };
}
