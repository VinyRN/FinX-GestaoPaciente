using FinX.Core.Entity;

namespace FinX.Core.Interface.Service
{
    public interface IGrupoPacienteHospitalService
    {
        Task<GrupoPacienteHospital> CreateAsync(GrupoPacienteHospital grupo);
        Task<GrupoPacienteHospital?> GetAsync(Guid id);
        Task<IEnumerable<GrupoPacienteHospital>> GetByPacienteAsync(Guid pacienteId);
        Task<IEnumerable<GrupoPacienteHospital>> GetByHospitalAsync(Guid hospitalId);
        Task<GrupoPacienteHospital?> GetByPacienteHospitalAsync(Guid pacienteId, Guid hospitalId);
        Task<bool> UpdateAsync(GrupoPacienteHospital grupo);
        Task<bool> DeleteAsync(Guid id);
    }
}
