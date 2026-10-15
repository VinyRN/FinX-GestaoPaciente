using FinX.Core.Entity;
using FinX.Core.Interface.Repository;
using MongoDB.Driver;

namespace FinX.Data
{
    public class MongoGrupoPacienteHospitalRepository : IGrupoPacienteHospitalRepository
    {
        private readonly IMongoCollection<GrupoPacienteHospital> _grupos;

        public MongoGrupoPacienteHospitalRepository(IMongoDatabase db)
        {
            _grupos = db.GetCollection<GrupoPacienteHospital>("grupospacientehospital");
        }

        public async Task<GrupoPacienteHospital> CreateAsync(GrupoPacienteHospital grupo)
        {
            grupo.Id = Guid.NewGuid();
            await _grupos.InsertOneAsync(grupo);
            return grupo;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var res = await _grupos.DeleteOneAsync(g => g.Id == id);
            return res.DeletedCount > 0;
        }

        public async Task<GrupoPacienteHospital?> GetAsync(Guid id)
        {
            return await _grupos.Find(g => g.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<GrupoPacienteHospital>> GetByHospitalAsync(Guid hospitalId)
        {
            return await _grupos.Find(g => g.HospitalId == hospitalId).ToListAsync();
        }

        public async Task<IEnumerable<GrupoPacienteHospital>> GetByPacienteAsync(Guid pacienteId)
        {
            return await _grupos.Find(g => g.PacienteId == pacienteId).ToListAsync();
        }

        public async Task<GrupoPacienteHospital?> GetByPacienteHospitalAsync(Guid pacienteId, Guid hospitalId)
        {
            return await _grupos.Find(g => g.PacienteId == pacienteId && g.HospitalId == hospitalId).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(GrupoPacienteHospital grupo)
        {
            var res = await _grupos.ReplaceOneAsync(g => g.Id == grupo.Id, grupo);
            return res.ModifiedCount > 0 || res.MatchedCount > 0;
        }
    }

}
