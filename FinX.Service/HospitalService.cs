using FinX.Core.Entity;
using FinX.Core.Interface.Repository;
using FinX.Core.Interface.Service;

namespace FinX.Service;

public sealed class HospitalService : IHospitalService
{
    private readonly IHospitalRepository _repository;

    public HospitalService(IHospitalRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Hospital>> GetAllAsync() => _repository.GetAllAsync();
    public Task<Hospital> CreateAsync(Hospital hospital) => _repository.CreateAsync(hospital);
    public Task<Hospital?> GetAsync(Guid id) => _repository.GetAsync(id);
    public Task<bool> UpdateAsync(Hospital hospital) => _repository.UpdateAsync(hospital);
    public Task<bool> DeleteAsync(Guid id) => _repository.DeleteAsync(id);
    public Task<Hospital?> GetByCnpjAsync(string cnpj) => _repository.GetByCnpjAsync(cnpj);
}
