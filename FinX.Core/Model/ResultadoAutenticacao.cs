namespace FinX.Core.Model;

public sealed class ResultadoAutenticacao
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiraEm { get; init; }
    public string NomeUsuario { get; init; } = string.Empty;
    public string Perfil { get; init; } = string.Empty;
}
