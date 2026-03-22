using CognitoTokenLambda.Configuration;

namespace CognitoTokenLambda.Interfaces;

public interface ISecretService
{
    Task<CognitoSettings> GetCognitoSettingsAsync();
}
