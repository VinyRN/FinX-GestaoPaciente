using FinX.Core.Entity;
using FinX.Core.Interface.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinX.Data
{
    public class MongoPacienteUnificacaoRepository : IPacienteUnificacaoRepository
    {
        private readonly IMongoCollection<Paciente> _patients;
        private readonly IMongoCollection<GrupoPacienteHospital> _grupos;
        private readonly IMongoCollection<Agendamento> _agendamentos;
        private readonly IMongoCollection<RegistroMedico> _records;
        private readonly ILogger<MongoPacienteUnificacaoRepository> _logger;

        public MongoPacienteUnificacaoRepository(
            IMongoDatabase db,
            ILogger<MongoPacienteUnificacaoRepository> logger)
        {
            _patients = db.GetCollection<Paciente>("patients");
            _grupos = db.GetCollection<GrupoPacienteHospital>("grupospacientehospital");
            _agendamentos = db.GetCollection<Agendamento>("agendamentos");
            _records = db.GetCollection<RegistroMedico>("medicalrecords");
            _logger = logger;
        }

        public async Task<IReadOnlyList<Paciente>> ObterDuplicadosPorCpfAsync()
        {
            var pipeline = new[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    ["_id"] = "$CPF",
                    ["count"] = new BsonDocument("$sum", 1),
                    ["patients"] = new BsonDocument("$push", "$$ROOT")
                }),
                new BsonDocument("$match", new BsonDocument("count", new BsonDocument("$gt", 1))),
                new BsonDocument("$unwind", "$patients"),
                new BsonDocument("$replaceRoot", new BsonDocument("newRoot", "$patients"))
            };

            return await _patients.Aggregate<Paciente>(pipeline).ToListAsync();
        }

        public async Task<IReadOnlyList<Paciente>> ObterDuplicadosPorCpfAsync(string cpf)
        {
            return await _patients.Find(p => p.Cpf == cpf).ToListAsync();
        }

        public async Task<ResultadoUnificacaoPaciente> UnificarDuplicadosPorCpfAsync()
        {
            var result = new ResultadoUnificacaoPaciente();

            try
            {
                var duplicateCpfs = await GetDuplicateCpfsAsync();

                foreach (var cpf in duplicateCpfs)
                {
                    var cpfResult = await UnificarDuplicadosPorCpfAsync(cpf);
                    result.TotalDuplicadosEncontrados += cpfResult.TotalDuplicadosEncontrados;
                    result.PacientesRemovidos += cpfResult.PacientesRemovidos;
                    result.PacientesMantidos += cpfResult.PacientesMantidos;
                    result.GruposAtualizados += cpfResult.GruposAtualizados;
                    result.AgendamentosAtualizados += cpfResult.AgendamentosAtualizados;
                    result.CpfsProcessados.AddRange(cpfResult.CpfsProcessados);
                    result.Erros.AddRange(cpfResult.Erros);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during patient unification process");
                result.Erros.Add($"Erro geral: {ex.Message}");
            }

            return result;
        }

        public async Task<ResultadoUnificacaoPaciente> UnificarDuplicadosPorCpfAsync(string cpf)
        {
            var result = new ResultadoUnificacaoPaciente();

            try
            {
                var duplicatePatients = await ObterDuplicadosPorCpfAsync(cpf);

                if (duplicatePatients.Count <= 1)
                {
                    return result;
                }

                result.TotalDuplicadosEncontrados = duplicatePatients.Count;
                result.CpfsProcessados.Add(cpf);

                var patientToKeep = duplicatePatients.OrderByDescending(p => p.DataCadastro).First();
                var patientsToRemove = duplicatePatients.Where(p => p.Id != patientToKeep.Id).ToList();

                _logger.LogInformation($"Unifying {duplicatePatients.Count} patients with CPF {cpf}. Keeping patient {patientToKeep.Id} (registered on {patientToKeep.DataCadastro})");

                foreach (var patientToRemove in patientsToRemove)
                {
                    var groupsFilter = Builders<GrupoPacienteHospital>.Filter.Eq(g => g.PacienteId, patientToRemove.Id);
                    var groupsUpdate = Builders<GrupoPacienteHospital>.Update.Set(g => g.PacienteId, patientToKeep.Id);
                    var groupsResult = await _grupos.UpdateManyAsync(groupsFilter, groupsUpdate);
                    result.GruposAtualizados += (int)groupsResult.ModifiedCount;

                    var appointmentsFilter = Builders<Agendamento>.Filter.Eq(a => a.PacienteId, patientToRemove.Id);
                    var appointmentsUpdate = Builders<Agendamento>.Update.Set(a => a.PacienteId, patientToKeep.Id);
                    var appointmentsResult = await _agendamentos.UpdateManyAsync(appointmentsFilter, appointmentsUpdate);
                    result.AgendamentosAtualizados += (int)appointmentsResult.ModifiedCount;

                    var recordsFilter = Builders<RegistroMedico>.Filter.Eq(r => r.PacienteId, patientToRemove.Id);
                    var recordsUpdate = Builders<RegistroMedico>.Update.Set(r => r.PacienteId, patientToKeep.Id);
                    await _records.UpdateManyAsync(recordsFilter, recordsUpdate);

                    await _patients.DeleteOneAsync(p => p.Id == patientToRemove.Id);
                    result.PacientesRemovidos++;

                    _logger.LogInformation($"Removed duplicate patient {patientToRemove.Id} and updated related records");
                }

                result.PacientesMantidos = 1;

                await RemoveDuplicateGruposForPatientAsync(patientToKeep.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error unifying patients with CPF {cpf}");
                result.Erros.Add($"Erro ao processar CPF {cpf}: {ex.Message}");
            }

            return result;
        }

        private async Task<List<string>> GetDuplicateCpfsAsync()
        {
            var pipeline = new[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    ["_id"] = "$CPF",
                    ["count"] = new BsonDocument("$sum", 1)
                }),
                new BsonDocument("$match", new BsonDocument("count", new BsonDocument("$gt", 1)))
            };

            var results = await _patients.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => doc["_id"].AsString).ToList();
        }

        private async Task RemoveDuplicateGruposForPatientAsync(Guid patientId)
        {
            var grupos = await _grupos.Find(g => g.PacienteId == patientId).ToListAsync();
            var uniqueGroups = grupos
                .GroupBy(g => new { g.PacienteId, g.HospitalId })
                .Where(group => group.Count() > 1)
                .ToList();

            foreach (var group in uniqueGroups)
            {
                var groupsToKeep = group.OrderByDescending(g => g.Id).First();
                var groupsToRemove = group.Where(g => g.Id != groupsToKeep.Id).ToList();

                foreach (var groupToRemove in groupsToRemove)
                {
                    await _grupos.DeleteOneAsync(g => g.Id == groupToRemove.Id);
                }
            }
        }
    }
}
