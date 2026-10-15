using FinX.Core.Entity;
using FinX.Core.Interface.Repository;
using FinX.Core.Interface.Service;
namespace FinX.Service;

public sealed class PacienteUnificacaoService : IPacienteUnificacaoService
{
    private readonly IPacienteUnificacaoRepository _repository;
    public PacienteUnificacaoService(IPacienteUnificacaoRepository repository) => _repository = repository;
    public Task<ResultadoUnificacaoPaciente> UnificarDuplicadosPorCpfAsync() => _repository.UnificarDuplicadosPorCpfAsync();
    public Task<ResultadoUnificacaoPaciente> UnificarDuplicadosPorCpfAsync(string cpf) => _repository.UnificarDuplicadosPorCpfAsync(cpf);
    public Task<IReadOnlyList<Paciente>> ObterDuplicadosPorCpfAsync() => _repository.ObterDuplicadosPorCpfAsync();
    public Task<IReadOnlyList<Paciente>> ObterDuplicadosPorCpfAsync(string cpf) => _repository.ObterDuplicadosPorCpfAsync(cpf);
}
