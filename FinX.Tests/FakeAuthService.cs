using FinX.Core.Interface.Service;
using FinX.Core.Model;

namespace FinX.Tests;

public sealed class FakeAuthService : IAuthService
{
    public Task<ResultadoAutenticacao?> AutenticarAsync(string nomeUsuario, string senha, CancellationToken cancellationToken = default)
    {
        if (nomeUsuario != "admin" || senha != "password")
        {
            return Task.FromResult<ResultadoAutenticacao?>(null);
        }

        return Task.FromResult<ResultadoAutenticacao?>(new ResultadoAutenticacao
        {
            Token = "fake-token",
            ExpiraEm = DateTime.UtcNow.AddHours(1),
            NomeUsuario = nomeUsuario,
            Perfil = "admin"
        });
    }
}
