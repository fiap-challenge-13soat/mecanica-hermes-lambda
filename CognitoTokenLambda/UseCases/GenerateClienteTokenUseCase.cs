using CognitoTokenLambda.Interfaces;
using CognitoTokenLambda.Models;

namespace CognitoTokenLambda.UseCases;

internal class GenerateClienteTokenUseCase(
    IClienteRepository clienteRepository,
    ICognitoTokenService cognitoTokenService,
    IEmailService emailService)
{
    public async Task<TokenResponse> ExecuteAsync(string cpf)
    {
        var cliente = await clienteRepository.GetByIdentificacaoFiscalAsync(cpf);
        if (cliente is null)
            return new TokenResponse
            {
                Message = "Não foi possível gerar o seu token. Erro: Cliente não encontrado para o CPF informado."
            };

        var token = await cognitoTokenService.GetTokenAsync();

        await emailService.SendTokenEmailAsync(cliente.Email, cliente.Nome, token);

        return new TokenResponse
        {
            Message = "O token foi gerado e enviado para seu e-mail cadastrado."
        };
    }
}
