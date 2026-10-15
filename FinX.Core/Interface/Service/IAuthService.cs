using FinX.Core.Model;

namespace FinX.Core.Interface.Service;

public interface IAuthService
{
    Task<ResultadoAutenticacao?> AutenticarAsync(string nomeUsuario, string senha, CancellationToken cancellationToken = default);
}
