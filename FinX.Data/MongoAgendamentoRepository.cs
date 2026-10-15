using FinX.Core.Entity;
using FinX.Core.Interface.Repository;
using MongoDB.Driver;

namespace FinX.Data
{
    public class MongoAgendamentoRepository : IAgendamentoRepository
    {
        private readonly IMongoCollection<Agendamento> _agendamentos;

        public MongoAgendamentoRepository(IMongoDatabase db)
        {
            _agendamentos = db.GetCollection<Agendamento>("agendamentos");
        }

        public async Task<Agendamento> CreateAsync(Agendamento agendamento)
        {
            agendamento.Id = Guid.NewGuid();
            await _agendamentos.InsertOneAsync(agendamento);
            return agendamento;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var res = await _agendamentos.DeleteOneAsync(a => a.Id == id);
            return res.DeletedCount > 0;
        }

        public async Task<IEnumerable<Agendamento>> GetAllAsync()
        {
            return await _agendamentos.Find(_ => true).ToListAsync();
        }

        public async Task<Agendamento?> GetAsync(Guid id)
        {
            return await _agendamentos.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Agendamento>> GetByHospitalAsync(Guid hospitalId)
        {
            return await _agendamentos.Find(a => a.HospitalId == hospitalId).ToListAsync();
        }

        public async Task<IEnumerable<Agendamento>> GetByPacienteAsync(Guid pacienteId)
        {
            return await _agendamentos.Find(a => a.PacienteId == pacienteId).ToListAsync();
        }

        public async Task<bool> UpdateAsync(Agendamento agendamento)
        {
            var res = await _agendamentos.ReplaceOneAsync(a => a.Id == agendamento.Id, agendamento);
            return res.ModifiedCount > 0 || res.MatchedCount > 0;
        }
    }

}
