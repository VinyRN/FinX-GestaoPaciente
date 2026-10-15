using FinX.Core.Entity;
using FinX.Core.Interface.Repository;
using FinX.Core.Model;

namespace FinX.Tests.Fakes;

public sealed class FakePacienteRepository : IPacienteRepository
{
    private readonly List<Paciente> _pacientes;
    private readonly List<RegistroMedico> _registros = [];

    public FakePacienteRepository(IEnumerable<Paciente>? pacientes = null)
    {
        _pacientes = pacientes?.ToList() ?? [];
    }

    public Task<Paciente> CriarAsync(Paciente paciente, CancellationToken cancellationToken = default)
    {
        paciente.Id = paciente.Id == Guid.Empty ? Guid.NewGuid() : paciente.Id;
        _pacientes.Add(paciente);
        return Task.FromResult(paciente);
    }

    public Task<Paciente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_pacientes.SingleOrDefault(paciente => paciente.Id == id));

    public Task<ResultadoPaginado<Paciente>> ConsultarAsync(FiltroPaciente filtro, CancellationToken cancellationToken = default)
    {
        var consulta = _pacientes.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            consulta = consulta.Where(paciente => paciente.Nome.Contains(filtro.Nome, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Cpf))
        {
            consulta = consulta.Where(paciente => paciente.Cpf == filtro.Cpf);
        }

        var filtrados = consulta.OrderBy(paciente => paciente.Nome).ThenBy(paciente => paciente.Id).ToList();
        return Task.FromResult(new ResultadoPaginado<Paciente>
        {
            Itens = filtrados.Skip((filtro.Pagina - 1) * filtro.TamanhoPagina).Take(filtro.TamanhoPagina).ToList(),
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalRegistros = filtrados.Count
        });
    }

    public Task<bool> AtualizarAsync(Paciente paciente, CancellationToken cancellationToken = default)
    {
        var indice = _pacientes.FindIndex(item => item.Id == paciente.Id);
        if (indice < 0)
        {
            return Task.FromResult(false);
        }

        _pacientes[indice] = paciente;
        return Task.FromResult(true);
    }

    public Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_pacientes.RemoveAll(paciente => paciente.Id == id) > 0);

    public Task<RegistroMedico> AdicionarRegistroMedicoAsync(Guid pacienteId, RegistroMedico registro, CancellationToken cancellationToken = default)
    {
        registro.Id = Guid.NewGuid();
        registro.PacienteId = pacienteId;
        _registros.Add(registro);
        return Task.FromResult(registro);
    }

    public Task<IReadOnlyList<RegistroMedico>> ObterHistoricoMedicoAsync(Guid pacienteId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<RegistroMedico>>(_registros.Where(registro => registro.PacienteId == pacienteId).ToList());
}
