using System.Text.Json;
using CognitoTokenLambda.Interfaces;
using CognitoTokenLambda.Models;

namespace CognitoTokenLambda.Infrastructure.Cognito;

internal class CognitoTokenService(ISecretService secretService) : ICognitoTokenService
{
    private static readonly HttpClient HttpClient = new();

    public async Task<string> GetTokenAsync()
    {
        var settings = await secretService.GetCognitoSettingsAsync();

        var tokenUrl = $"{settings.Domain}/oauth2/token";

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = settings.ClientId,
            ["client_secret"] = settings.ClientSecret,
            ["scope"] = settings.Scope
        });

        var response = await HttpClient.PostAsync(tokenUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Falha ao obter token do Cognito. Status: {response.StatusCode}. Detalhes: {errorBody}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<CognitoTokenResponse>(responseBody)
                            ?? throw new InvalidOperationException("Falha ao deserializar a resposta do Cognito.");

        return tokenResponse.AccessToken;
    }
}
