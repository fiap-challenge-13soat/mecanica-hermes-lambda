using System.Text.Json.Serialization;

namespace CognitoTokenLambda.Models;

internal class CognitoTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = default!;
}
