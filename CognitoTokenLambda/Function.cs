using Amazon.Lambda.Core;
using CognitoTokenLambda.Models;
using CognitoTokenLambda.UseCases;
using Microsoft.Extensions.DependencyInjection;

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
    /// Lambda handler that receives a CPF, looks up the client, generates a Cognito token,
    /// and sends it via email.
    /// </summary>
    /// <param name="request">Request containing the client CPF.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns>A TokenResponse with a success or failure message.</returns>
    public async Task<TokenResponse> FunctionHandler(TokenRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Iniciando geração de token para CPF: {MaskCpf(request.Cpf)}");

        if (string.IsNullOrWhiteSpace(request.Cpf))
        {
            context.Logger.LogWarning("CPF não informado na requisição.");
            return new TokenResponse
            {
                Message = "Não foi possível gerar o seu token. Erro: CPF é obrigatório."
            };
        }

        try
        {
            var useCase = _serviceProvider.GetRequiredService<GenerateClienteTokenUseCase>();
            var response = await useCase.ExecuteAsync(request.Cpf);

            context.Logger.LogInformation($"Resultado: {response.Message}");
            return response;
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Erro ao gerar token: {ex.Message}");
            return new TokenResponse
            {
                Message = $"Não foi possível gerar o seu token. Erro: {ex.Message}"
            };
        }
    }

    private static string MaskCpf(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) || cpf.Length < 4)
            return "***";

        return $"***{cpf[^4..]}";
    }
}