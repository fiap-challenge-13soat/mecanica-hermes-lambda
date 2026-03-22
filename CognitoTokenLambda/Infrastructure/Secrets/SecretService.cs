using System.Text.Json;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using CognitoTokenLambda.Configuration;
using CognitoTokenLambda.Interfaces;

namespace CognitoTokenLambda.Infrastructure.Secrets;

internal class SecretService : ISecretService
{
    private static readonly AmazonSecretsManagerClient Client = new();

    private static readonly string SecretId =
        Environment.GetEnvironmentVariable("COGNITO_SECRET_ID") ?? "CognitoTokenLambda";

    private CognitoSettings? _cachedSettings;

    public async Task<CognitoSettings> GetCognitoSettingsAsync()
    {
        if (_cachedSettings is not null)
            return _cachedSettings;

        var request = new GetSecretValueRequest
        {
            SecretId = SecretId,
            VersionStage = "AWSCURRENT"
        };

        var response = await Client.GetSecretValueAsync(request);

        if (string.IsNullOrEmpty(response.SecretString))
            throw new InvalidOperationException("SecretString retornado vazio do AWS Secrets Manager.");

        _cachedSettings = JsonSerializer.Deserialize<CognitoSettings>(response.SecretString)
                          ?? throw new InvalidOperationException("Falha ao deserializar as configurações do Cognito.");

        return _cachedSettings;
    }
}
