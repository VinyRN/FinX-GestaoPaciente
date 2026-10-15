using System.ComponentModel.DataAnnotations;

namespace FinX.Core.Dto.Request.Autenticacao;

public sealed class LoginRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
