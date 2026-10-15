using FinX.Core.Entity;
using FinX.Core.Interface.Repository;

namespace FinX.Tests.Fakes;

public sealed class FakeUsuarioRepository : IUsuarioRepository
{
    private readonly List<Usuario> _usuarios = [];

    public FakeUsuarioRepository(IEnumerable<Usuario>? usuarios = null)
    {
        if (usuarios is not null)
        {
            _usuarios.AddRange(usuarios);
        }
    }

    public Task<Usuario?> ObterPorNomeUsuarioAsync(string nomeUsuario, CancellationToken cancellationToken = default) =>
        Task.FromResult(_usuarios.SingleOrDefault(usuario => usuario.NomeUsuario == nomeUsuario));

    public Task<Usuario> CriarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        _usuarios.Add(usuario);
        return Task.FromResult(usuario);
    }
}
