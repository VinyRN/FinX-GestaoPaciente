using FinX.Core.Dto.Request.Autenticacao;
using FinX.Core.Dto.Response.Autenticacao;
using FinX.Core.Interface.Service;
using Microsoft.AspNetCore.Mvc;

namespace FinX.Api.Controllers;

[ApiController]
[Route("api/Auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var resultado = await _authService.AutenticarAsync(request.Username, request.Password, cancellationToken);
        if (resultado is null)
        {
            return Unauthorized(new { mensagem = "Usuário ou senha inválidos." });
        }

        return Ok(new LoginResponseDto
        {
            Token = resultado.Token,
            ExpiraEm = resultado.ExpiraEm,
            NomeUsuario = resultado.NomeUsuario,
            Perfil = resultado.Perfil
        });
    }
}
