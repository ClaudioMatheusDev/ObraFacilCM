namespace ObraFacil.Application.Interfaces;

/// <summary>Serviço de autenticação local baseado em senha armazenada nas configurações.</summary>
public interface IAuthService
{
    /// <summary>Retorna true se uma senha de acesso está definida.</summary>
    Task<bool> TemSenhaDefinidaAsync(CancellationToken ct = default);

    /// <summary>Verifica se a senha informada é válida. Retorna true se não há senha definida.</summary>
    Task<bool> VerificarSenhaAsync(string senha, CancellationToken ct = default);

    /// <summary>Define ou altera a senha de acesso (mínimo 4 caracteres).</summary>
    Task DefinirSenhaAsync(string novaSenha, CancellationToken ct = default);

    /// <summary>Remove a senha de acesso, deixando o sistema sem proteção.</summary>
    Task RemoverSenhaAsync(CancellationToken ct = default);
}
