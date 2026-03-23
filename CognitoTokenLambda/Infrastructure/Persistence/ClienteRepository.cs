using System.Text.RegularExpressions;
using CognitoTokenLambda.Configuration;
using CognitoTokenLambda.Interfaces;
using CognitoTokenLambda.Models;
using Npgsql;

namespace CognitoTokenLambda.Infrastructure.Persistence;

internal partial class ClienteRepository(RdsSettings rdsSettings, ISecretService secretService) : IClienteRepository
{
    private const string Query = """
        SELECT "NomeCivil", "NomeSocial", "Email"
        FROM "Clientes"
        WHERE "IdentificacaoFiscal" = @cpf AND "IsDeleted" = false
        LIMIT 1
        """;

    public async Task<ClienteInfo?> GetByIdentificacaoFiscalAsync(string cpf)
    {
        var cpfNormalizado = NormalizarNumeros(cpf);

        await using var connection = await CreateConnectionAsync();
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(Query, connection);
        command.Parameters.AddWithValue("@cpf", cpfNormalizado);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        var nomeCivil = reader.GetString(0);
        var nomeSocial = reader.IsDBNull(1) ? null : reader.GetString(1);
        var email = reader.GetString(2);

        var nome = nomeSocial ?? nomeCivil;

        return new ClienteInfo(nome, email);
    }

    private async Task<NpgsqlConnection> CreateConnectionAsync()
    {
        var cognitoSettings = await secretService.GetCognitoSettingsAsync();
        
        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = rdsSettings.Host,
            Port = rdsSettings.Port,
            Database = rdsSettings.Database,
            Username = rdsSettings.Username,
            Password = cognitoSettings.DbPassword,
            SslMode = SslMode.Require
        }.ToString();

        return new NpgsqlConnection(connectionString);
    }

    private static string NormalizarNumeros(string valor)
    {
        return SomenteNumerosRegex().Replace(valor, "");
    }

    [GeneratedRegex(@"\D", RegexOptions.None, 1000)]
    private static partial Regex SomenteNumerosRegex();
}
