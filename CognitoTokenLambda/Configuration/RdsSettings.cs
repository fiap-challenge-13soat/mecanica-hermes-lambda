namespace CognitoTokenLambda.Configuration;

public class RdsSettings
{
    public string Host { get; init; } = default!;
    public int Port { get; init; }
    public string Database { get; init; } = default!;
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;

    public static RdsSettings FromEnvironment() => new()
    {
        Host = Environment.GetEnvironmentVariable("RDS__Host")
               ?? throw new InvalidOperationException("A variável de ambiente 'RDS__Host' não foi configurada."),
        Port = int.TryParse(Environment.GetEnvironmentVariable("RDS__Port"), out var port) ? port : 5432,
        Database = Environment.GetEnvironmentVariable("RDS__Database")
                   ?? throw new InvalidOperationException("A variável de ambiente 'RDS__Database' não foi configurada."),
        Username = Environment.GetEnvironmentVariable("RDS__Username")
                   ?? throw new InvalidOperationException("A variável de ambiente 'RDS__Username' não foi configurada."),
        Password = Environment.GetEnvironmentVariable("RDS__Password")
                   ?? throw new InvalidOperationException("A variável de ambiente 'RDS__Password' não foi configurada.")
    };
}
