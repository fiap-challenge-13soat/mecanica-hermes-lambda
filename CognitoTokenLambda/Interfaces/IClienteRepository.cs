using CognitoTokenLambda.Models;

namespace CognitoTokenLambda.Interfaces;

public interface IClienteRepository
{
    Task<ClienteInfo?> GetByIdentificacaoFiscalAsync(string cpf);
}
