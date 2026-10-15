using FinX.Core.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FinX.Api.Controllers;

[ApiController]
[Route("api/ExternalExams")]
[Authorize]
public class ExamesExternosController : ControllerBase
{
    private readonly IExamesExternosService _service;
    public ExamesExternosController(IExamesExternosService service) => _service = service;

    [HttpGet("{cpf}")]
    public IActionResult ConsultarPorCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return BadRequest(new { mensagem = "CPF é obrigatório." });
        return Ok(_service.ConsultarPorCpf(cpf));
    }
}
