using FinX.Core.Entity;

namespace FinX.Core.Interface.Repository;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorNomeUsuarioAsync(string nomeUsuario, CancellationToken cancellationToken = default);
    Task<Usuario> CriarAsync(Usuario usuario, CancellationToken cancellationToken = default);
}
