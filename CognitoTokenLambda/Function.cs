using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CognitoTokenLambda.Models;
using CognitoTokenLambda.UseCases;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CognitoTokenLambda;

public class Function
{
    private readonly ServiceProvider _serviceProvider;

    /// <summary>
    /// Default constructor. Called once during Lambda cold start.
    /// Initializes the DI container so services are reused across invocations.
    /// </summary>
    public Function()
    {
        _serviceProvider = Startup.ConfigureServices();
    }

    /// <summary>
    /// Constructor that accepts a pre-configured ServiceProvider (useful for testing).
    /// </summary>
    internal Function(ServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Lambda handler that receives a CPF via API Gateway, looks up the client, generates a Cognito token,
    /// and sends it via email.
    /// </summary>
    /// <param name="apiRequest">API Gateway HTTP API request.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns>API Gateway HTTP API response with statusCode, headers, and body.</returns>
    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest apiRequest, ILambdaContext context)
    {
        try
        {
            // Parse the request body
            if (string.IsNullOrWhiteSpace(apiRequest.Body))
            {
                context.Logger.LogWarning("Request body is empty.");
                return CreateErrorResponse(400, "Request body is required.");
            }

            TokenRequest? request;
            try
            {
                request = JsonSerializer.Deserialize<TokenRequest>(apiRequest.Body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                context.Logger.LogWarning($"Invalid JSON in request body: {ex.Message}");
                return CreateErrorResponse(400, "Invalid JSON format.");
            }

            if (request == null || string.IsNullOrWhiteSpace(request.Cpf))
            {
                context.Logger.LogWarning("CPF não informado na requisição.");
                return CreateErrorResponse(400, "CPF é obrigatório.");
            }

            context.Logger.LogInformation($"Iniciando geração de token para CPF: {MaskCpf(request.Cpf)}");

            var useCase = _serviceProvider.GetRequiredService<GenerateClienteTokenUseCase>();
            var response = await useCase.ExecuteAsync(request.Cpf);

            context.Logger.LogInformation($"Resultado: {response.Message}");
            
            return CreateSuccessResponse(response);
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Erro ao gerar token: {ex.Message}");
            return CreateErrorResponse(500, $"Não foi possível gerar o seu token. Erro: {ex.Message}");
        }
    }

    private static APIGatewayHttpApiV2ProxyResponse CreateSuccessResponse(TokenResponse response)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "*" }
            },
            Body = JsonSerializer.Serialize(response)
        };
    }

    private static APIGatewayHttpApiV2ProxyResponse CreateErrorResponse(int statusCode, string message)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = statusCode,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "*" }
            },
            Body = JsonSerializer.Serialize(new { message })
        };
    }

    private static string MaskCpf(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) || cpf.Length < 4)
            return "***";

        return $"***{cpf[^4..]}";
    }
}