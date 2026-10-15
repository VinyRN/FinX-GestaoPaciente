namespace FinX.Core.Dto.Response.Autenticacao;

public sealed class LoginResponseDto
{
    public string Token { get; init; } = string.Empty;
    public string Tipo { get; init; } = "Bearer";
    public DateTime ExpiraEm { get; init; }
    public string NomeUsuario { get; init; } = string.Empty;
    public string Perfil { get; init; } = string.Empty;
}
