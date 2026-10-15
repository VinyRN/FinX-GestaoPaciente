using FinX.Core.Entity;
using FinX.Core.Interface.Repository;
using MongoDB.Driver;

namespace FinX.Data.Repository;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly IMongoCollection<Usuario> _usuarios;

    public UsuarioRepository(IMongoDatabase database)
    {
        _usuarios = database.GetCollection<Usuario>("usuarios");
    }

    public async Task<Usuario?> ObterPorNomeUsuarioAsync(string nomeUsuario, CancellationToken cancellationToken = default) =>
        await _usuarios.Find(usuario => usuario.NomeUsuario == nomeUsuario).FirstOrDefaultAsync(cancellationToken);

    public async Task<Usuario> CriarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        usuario.Id = usuario.Id == Guid.Empty ? Guid.NewGuid() : usuario.Id;
        usuario.DataCadastro = usuario.DataCadastro == default ? DateTime.UtcNow : usuario.DataCadastro;
        await _usuarios.InsertOneAsync(usuario, cancellationToken: cancellationToken);
        return usuario;
    }
}
