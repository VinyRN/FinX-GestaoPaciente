using FinX.Core.Entity;

namespace FinX.Core.Interface.Service
{
    public interface IHospitalService
    {
        Task<Hospital> CreateAsync(Hospital hospital);
        Task<Hospital?> GetAsync(Guid id);
        Task<IEnumerable<Hospital>> GetAllAsync();
        Task<bool> UpdateAsync(Hospital hospital);
        Task<bool> DeleteAsync(Guid id);
        Task<Hospital?> GetByCnpjAsync(string cnpj);
    }
}
