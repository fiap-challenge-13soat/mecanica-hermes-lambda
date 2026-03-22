using CognitoTokenLambda.Configuration;
using CognitoTokenLambda.Infrastructure.Cognito;
using CognitoTokenLambda.Infrastructure.Email;
using CognitoTokenLambda.Infrastructure.Persistence;
using CognitoTokenLambda.Infrastructure.Secrets;
using CognitoTokenLambda.Interfaces;
using CognitoTokenLambda.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace CognitoTokenLambda;

public static class Startup
{
    public static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        var rdsSettings = RdsSettings.FromEnvironment();
        var emailSettings = EmailSettings.FromEnvironment();

        services.AddSingleton(rdsSettings);
        services.AddSingleton(emailSettings);
        services.AddSingleton<ISecretService, SecretService>();
        services.AddSingleton<ICognitoTokenService, CognitoTokenService>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<IClienteRepository, ClienteRepository>();
        services.AddTransient<GenerateClienteTokenUseCase>();

        return services.BuildServiceProvider();
    }
}
