using FinX.Api.Mapping;
using FinX.Core.Dto.Request.Paciente;
using FinX.Core.Entity;
using FinX.Core.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinX.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/Paciente")]
public sealed class PacienteController : ControllerBase
{
    private readonly IPacienteService _pacienteService;
    private readonly IPacienteUnificacaoService _unificacaoService;

    public PacienteController(IPacienteService pacienteService, IPacienteUnificacaoService unificacaoService)
    {
        _pacienteService = pacienteService;
        _unificacaoService = unificacaoService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] PacienteRequestDto request, CancellationToken cancellationToken)
    {
        if (request.DataNascimento.Date > DateTime.UtcNow.Date)
        {
            return BadRequest(new { mensagem = "Data de nascimento inválida." });
        }

        var paciente = request.ToEntity();
        paciente.Id = Guid.NewGuid();
        paciente.DataCadastro = DateTime.UtcNow;
        var criado = await _pacienteService.CriarAsync(paciente, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado.ToResponse());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        var paciente = await _pacienteService.ObterPorIdAsync(id, cancellationToken);
        return paciente is null ? NotFound() : Ok(paciente.ToResponse());
    }

    [HttpGet]
    public async Task<IActionResult> Consultar([FromQuery] PacienteFiltroRequestDto request, CancellationToken cancellationToken)
    {
        if (request.NascimentoDe > request.NascimentoAte)
        {
            return BadRequest(new { mensagem = "O início do período de nascimento deve ser anterior ao fim." });
        }

        var resultado = await _pacienteService.ConsultarAsync(request.ToFiltro(), cancellationToken);
        return Ok(resultado.ToResponse(item => item.ToResponse()));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] PacienteRequestDto request, CancellationToken cancellationToken)
    {
        var existente = await _pacienteService.ObterPorIdAsync(id, cancellationToken);
        if (existente is null)
        {
            return NotFound();
        }

        if (request.DataNascimento.Date > DateTime.UtcNow.Date)
        {
            return BadRequest(new { mensagem = "Data de nascimento inválida." });
        }

        var paciente = request.ToEntity();
        paciente.Id = id;
        paciente.DataCadastro = existente.DataCadastro;
        await _pacienteService.AtualizarAsync(paciente, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken cancellationToken) =>
        await _pacienteService.ExcluirAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpPost("{id:guid}/registros-medicos")]
    public async Task<IActionResult> AdicionarRegistroMedico(Guid id, [FromBody] RegistroMedicoRequestDto request, CancellationToken cancellationToken)
    {
        if (await _pacienteService.ObterPorIdAsync(id, cancellationToken) is null)
        {
            return NotFound();
        }

        var registro = new RegistroMedico
        {
            Data = request.Data,
            Tipo = request.Tipo.Trim(),
            Descricao = request.Descricao.Trim()
        };
        var criado = await _pacienteService.AdicionarRegistroMedicoAsync(id, registro, cancellationToken);
        return CreatedAtAction(nameof(ObterHistoricoMedico), new { id }, criado.ToResponse());
    }

    [HttpGet("{id:guid}/registros-medicos")]
    public async Task<IActionResult> ObterHistoricoMedico(Guid id, CancellationToken cancellationToken)
    {
        if (await _pacienteService.ObterPorIdAsync(id, cancellationToken) is null)
        {
            return NotFound();
        }

        var historico = await _pacienteService.ObterHistoricoMedicoAsync(id, cancellationToken);
        return Ok(historico.Select(registro => registro.ToResponse()));
    }

    [HttpPost("unificar-duplicados")]
    public async Task<IActionResult> UnificarDuplicados() => Ok(await _unificacaoService.UnificarDuplicadosPorCpfAsync());

    [HttpPost("unificar-duplicados/{cpf}")]
    public async Task<IActionResult> UnificarDuplicadosPorCpf(string cpf) => Ok(await _unificacaoService.UnificarDuplicadosPorCpfAsync(cpf));

    [HttpGet("duplicados")]
    public async Task<IActionResult> ObterDuplicados() => Ok((await _unificacaoService.ObterDuplicadosPorCpfAsync()).Select(paciente => paciente.ToResponse()));

    [HttpGet("duplicados/{cpf}")]
    public async Task<IActionResult> ObterDuplicadosPorCpf(string cpf) => Ok((await _unificacaoService.ObterDuplicadosPorCpfAsync(cpf)).Select(paciente => paciente.ToResponse()));
}
