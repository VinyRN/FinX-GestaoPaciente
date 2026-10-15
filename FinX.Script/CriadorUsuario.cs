using FinX.Core.Entity;
using FinX.Core.Interface.Repository;
using FinX.Core.Interface.Service;

namespace FinX.Scripts;

public sealed class CriadorUsuario
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICriptografiaSenha _criptografiaSenha;

    public CriadorUsuario(IUsuarioRepository usuarioRepository, ICriptografiaSenha criptografiaSenha)
    {
        _usuarioRepository = usuarioRepository;
        _criptografiaSenha = criptografiaSenha;
    }

    public async Task<Usuario> ExecutarAsync(
        string nomeUsuario,
        string senha,
        string perfil,
        CancellationToken cancellationToken = default)
    {
        nomeUsuario = nomeUsuario.Trim();
        if (nomeUsuario.Length < 3)
        {
            throw new ArgumentException("O nome do usuário deve ter ao menos 3 caracteres.");
        }

        if (senha.Length < 12)
        {
            throw new ArgumentException("A senha deve ter ao menos 12 caracteres.");
        }

        if (await _usuarioRepository.ObterPorNomeUsuarioAsync(nomeUsuario, cancellationToken) is not null)
        {
            throw new InvalidOperationException($"O usuário '{nomeUsuario}' já existe.");
        }

        var (hash, salt) = _criptografiaSenha.CriarHash(senha);
        return await _usuarioRepository.CriarAsync(new Usuario
        {
            NomeUsuario = nomeUsuario,
            SenhaHash = hash,
            SenhaSalt = salt,
            Perfil = string.IsNullOrWhiteSpace(perfil) ? "usuario" : perfil.Trim(),
            Ativo = true
        }, cancellationToken);
    }
}
