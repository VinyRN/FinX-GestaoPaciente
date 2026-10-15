using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FinX.Core.Interface.Repository;
using FinX.Core.Interface.Service;
using FinX.Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinX.Service;

public sealed class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICriptografiaSenha _criptografiaSenha;
    private readonly IKeyService _keyService;
    private readonly string _emissor;
    private readonly int _duracaoMinutos;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        ICriptografiaSenha criptografiaSenha,
        IKeyService keyService,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _criptografiaSenha = criptografiaSenha;
        _keyService = keyService;
        _emissor = configuration["Jwt:Issuer"] ?? "FinXApi";
        _duracaoMinutos = configuration.GetValue("Jwt:ExpirationMinutes", 360);
    }

    public async Task<ResultadoAutenticacao?> AutenticarAsync(string nomeUsuario, string senha, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterPorNomeUsuarioAsync(nomeUsuario.Trim(), cancellationToken);
        if (usuario is null || !usuario.Ativo || !_criptografiaSenha.Verificar(senha, usuario.SenhaHash, usuario.SenhaSalt))
        {
            return null;
        }

        var expiraEm = DateTime.UtcNow.AddMinutes(_duracaoMinutos);
        var chave = new RsaSecurityKey(_keyService.GetPrivateKey()) { KeyId = _keyService.GetKeyId() };
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.RsaSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.NomeUsuario),
            new Claim(ClaimTypes.Role, usuario.Perfil),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };
        var token = new JwtSecurityToken(
            issuer: _emissor,
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return new ResultadoAutenticacao
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiraEm = expiraEm,
            NomeUsuario = usuario.NomeUsuario,
            Perfil = usuario.Perfil
        };
    }
}
