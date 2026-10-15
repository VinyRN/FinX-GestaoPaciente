using FinX.Core.Entity;
using FinX.Core.Interface.Repository;
using FinX.Core.Interface.Service;

namespace FinX.Service;

public sealed class GrupoPacienteHospitalService : IGrupoPacienteHospitalService
{
    private readonly IGrupoPacienteHospitalRepository _repository;

    public GrupoPacienteHospitalService(IGrupoPacienteHospitalRepository repository)
    {
        _repository = repository;
    }

    public Task<GrupoPacienteHospital> CreateAsync(GrupoPacienteHospital grupo) => _repository.CreateAsync(grupo);
    public Task<GrupoPacienteHospital?> GetAsync(Guid id) => _repository.GetAsync(id);
    public Task<IEnumerable<GrupoPacienteHospital>> GetByPacienteAsync(Guid pacienteId) => _repository.GetByPacienteAsync(pacienteId);
    public Task<IEnumerable<GrupoPacienteHospital>> GetByHospitalAsync(Guid hospitalId) => _repository.GetByHospitalAsync(hospitalId);
    public Task<GrupoPacienteHospital?> GetByPacienteHospitalAsync(Guid pacienteId, Guid hospitalId) => _repository.GetByPacienteHospitalAsync(pacienteId, hospitalId);
    public Task<bool> UpdateAsync(GrupoPacienteHospital grupo) => _repository.UpdateAsync(grupo);
    public Task<bool> DeleteAsync(Guid id) => _repository.DeleteAsync(id);
}
