using FinX.Core.Entity;

namespace FinX.Core.Interface.Service;

public interface IPacienteUnificacaoService
{
    Task<ResultadoUnificacaoPaciente> UnificarDuplicadosPorCpfAsync();
    Task<ResultadoUnificacaoPaciente> UnificarDuplicadosPorCpfAsync(string cpf);
    Task<IReadOnlyList<Paciente>> ObterDuplicadosPorCpfAsync();
    Task<IReadOnlyList<Paciente>> ObterDuplicadosPorCpfAsync(string cpf);
}
