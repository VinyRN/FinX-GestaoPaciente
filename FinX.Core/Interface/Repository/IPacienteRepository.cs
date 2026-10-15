using FinX.Core.Entity;
using FinX.Core.Model;

namespace FinX.Core.Interface.Repository;

public interface IPacienteRepository
{
    Task<Paciente> CriarAsync(Paciente paciente, CancellationToken cancellationToken = default);
    Task<Paciente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoPaginado<Paciente>> ConsultarAsync(FiltroPaciente filtro, CancellationToken cancellationToken = default);
    Task<bool> AtualizarAsync(Paciente paciente, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RegistroMedico> AdicionarRegistroMedicoAsync(Guid pacienteId, RegistroMedico registro, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroMedico>> ObterHistoricoMedicoAsync(Guid pacienteId, CancellationToken cancellationToken = default);
}
