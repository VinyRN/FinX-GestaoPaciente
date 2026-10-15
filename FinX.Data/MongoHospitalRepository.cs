using FinX.Core.Entity;
using FinX.Core.Interface.Repository;
using MongoDB.Driver;

namespace FinX.Data
{
    public class MongoHospitalRepository : IHospitalRepository
    {
        private readonly IMongoCollection<Hospital> _hospitals;

        public MongoHospitalRepository(IMongoDatabase db)
        {
            _hospitals = db.GetCollection<Hospital>("hospitals");
        }

        public async Task<Hospital> CreateAsync(Hospital hospital)
        {
            hospital.Id = Guid.NewGuid();
            await _hospitals.InsertOneAsync(hospital);
            return hospital;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var res = await _hospitals.DeleteOneAsync(h => h.Id == id);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Hospital>> GetAllAsync()
        {
            return await _hospitals.Find(_ => true).ToListAsync();
        }

        public async Task<Hospital?> GetAsync(Guid id)
        {
            return await _hospitals.Find(h => h.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Hospital?> GetByCnpjAsync(string cnpj)
        {
            return await _hospitals.Find(h => h.Cnpj == cnpj).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(Hospital hospital)
        {
            var res = await _hospitals.ReplaceOneAsync(h => h.Id == hospital.Id, hospital);
            return res.ModifiedCount > 0 || res.MatchedCount > 0;
        }
    }

}
