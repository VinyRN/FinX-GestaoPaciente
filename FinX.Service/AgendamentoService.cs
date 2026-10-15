using FinX.Core.Entity;
using FinX.Core.Interface.Repository;
using FinX.Core.Interface.Service;

namespace FinX.Service;

public sealed class AgendamentoService : IAgendamentoService
{
    private readonly IAgendamentoRepository _repository;

    public AgendamentoService(IAgendamentoRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Agendamento>> GetAllAsync() => _repository.GetAllAsync();
    public Task<Agendamento> CreateAsync(Agendamento agendamento) => _repository.CreateAsync(agendamento);
    public Task<Agendamento?> GetAsync(Guid id) => _repository.GetAsync(id);
    public Task<IEnumerable<Agendamento>> GetByPacienteAsync(Guid pacienteId) => _repository.GetByPacienteAsync(pacienteId);
    public Task<IEnumerable<Agendamento>> GetByHospitalAsync(Guid hospitalId) => _repository.GetByHospitalAsync(hospitalId);
    public Task<bool> UpdateAsync(Agendamento agendamento) => _repository.UpdateAsync(agendamento);
    public Task<bool> DeleteAsync(Guid id) => _repository.DeleteAsync(id);
}
