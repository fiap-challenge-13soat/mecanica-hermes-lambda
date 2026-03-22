namespace CognitoTokenLambda.Interfaces;

public interface ICognitoTokenService
{
    Task<string> GetTokenAsync();
}
