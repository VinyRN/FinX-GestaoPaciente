using FinX.Core.Entity;

namespace FinX.Core.Interface.Service
{
    public interface IAgendamentoService
    {
        Task<Agendamento> CreateAsync(Agendamento agendamento);
        Task<Agendamento?> GetAsync(Guid id);
        Task<IEnumerable<Agendamento>> GetAllAsync();
        Task<IEnumerable<Agendamento>> GetByPacienteAsync(Guid pacienteId);
        Task<IEnumerable<Agendamento>> GetByHospitalAsync(Guid hospitalId);
        Task<bool> UpdateAsync(Agendamento agendamento);
        Task<bool> DeleteAsync(Guid id);
    }
}
