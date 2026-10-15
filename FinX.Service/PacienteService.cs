using FinX.Core.Entity;
using FinX.Core.Interface.Repository;
using FinX.Core.Interface.Service;
using FinX.Core.Model;

namespace FinX.Service;

public sealed class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _repository;

    public PacienteService(IPacienteRepository repository)
    {
        _repository = repository;
    }

    public Task<Paciente> CriarAsync(Paciente paciente, CancellationToken cancellationToken = default) => _repository.CriarAsync(paciente, cancellationToken);
    public Task<Paciente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) => _repository.ObterPorIdAsync(id, cancellationToken);
    public Task<ResultadoPaginado<Paciente>> ConsultarAsync(FiltroPaciente filtro, CancellationToken cancellationToken = default) => _repository.ConsultarAsync(filtro, cancellationToken);
    public Task<bool> AtualizarAsync(Paciente paciente, CancellationToken cancellationToken = default) => _repository.AtualizarAsync(paciente, cancellationToken);
    public Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default) => _repository.ExcluirAsync(id, cancellationToken);
    public Task<RegistroMedico> AdicionarRegistroMedicoAsync(Guid pacienteId, RegistroMedico registro, CancellationToken cancellationToken = default) => _repository.AdicionarRegistroMedicoAsync(pacienteId, registro, cancellationToken);
    public Task<IReadOnlyList<RegistroMedico>> ObterHistoricoMedicoAsync(Guid pacienteId, CancellationToken cancellationToken = default) => _repository.ObterHistoricoMedicoAsync(pacienteId, cancellationToken);
}
