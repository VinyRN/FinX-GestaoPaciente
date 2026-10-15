using FinX.Core.Entity;
namespace FinX.Core.Interface.Repository;

public interface IPacienteUnificacaoRepository
{
    Task<ResultadoUnificacaoPaciente> UnificarDuplicadosPorCpfAsync();
    Task<ResultadoUnificacaoPaciente> UnificarDuplicadosPorCpfAsync(string cpf);
    Task<IReadOnlyList<Paciente>> ObterDuplicadosPorCpfAsync();
    Task<IReadOnlyList<Paciente>> ObterDuplicadosPorCpfAsync(string cpf);
}
