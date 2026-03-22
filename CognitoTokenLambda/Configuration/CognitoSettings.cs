using System.Text.Json.Serialization;

namespace CognitoTokenLambda.Configuration;

public class CognitoSettings
{
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = default!;

    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; set; } = default!;

    [JsonPropertyName("domain")]
    public string Domain { get; set; } = default!;

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = default!;
}
