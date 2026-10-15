using FinX.Core.Entity;
using FinX.Service;
using FinX.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FinX.Tests;

public sealed class AutenticacaoServiceTests
{
    [Fact]
    public async Task AutenticarAsync_DeveGerarJwtParaCredenciaisValidas()
    {
        var criptografia = new CriptografiaSenha();
        var (hash, salt) = criptografia.CriarHash("SenhaForte123!");
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            NomeUsuario = "admin",
            SenhaHash = hash,
            SenhaSalt = salt,
            Perfil = "admin",
            Ativo = true
        };
        var configuracao = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Jwt:Issuer"] = "FinXApi" })
            .Build();
        var service = new AuthService(new FakeUsuarioRepository([usuario]), criptografia, new FakeKeyService(), configuracao);

        var resultado = await service.AutenticarAsync("admin", "SenhaForte123!");

        Assert.NotNull(resultado);
        Assert.Equal("admin", resultado.NomeUsuario);
        Assert.Equal("admin", resultado.Perfil);
        Assert.Equal(3, resultado.Token.Split('.').Length);
    }

    [Fact]
    public async Task AutenticarAsync_DeveRecusarSenhaInvalida()
    {
        var criptografia = new CriptografiaSenha();
        var (hash, salt) = criptografia.CriarHash("SenhaForte123!");
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            NomeUsuario = "admin",
            SenhaHash = hash,
            SenhaSalt = salt,
            Ativo = true
        };
        var service = new AuthService(
            new FakeUsuarioRepository([usuario]),
            criptografia,
            new FakeKeyService(),
            new ConfigurationBuilder().Build());

        var resultado = await service.AutenticarAsync("admin", "senha-errada");

        Assert.Null(resultado);
    }
}
