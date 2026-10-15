using FinX.Core.Entity;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinX.Scripts
{
    /// Script principal para unificação de pacientes duplicados
    public class UnifyDuplicatePatients
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<UnifyDuplicatePatients> _logger;

        private readonly IMongoCollection<Paciente> _patients;
        private readonly IMongoCollection<Hospital> _hospitals;
        private readonly IMongoCollection<GrupoPacienteHospital> _grupos;
        private readonly IMongoCollection<Agendamento> _agendamentos;
        private readonly IMongoCollection<RegistroMedico> _medicalRecords;

        public UnifyDuplicatePatients(IMongoDatabase database, ILogger<UnifyDuplicatePatients> logger)
        {
            _database = database;
            _logger = logger;

            _patients = _database.GetCollection<Paciente>("patients");
            _hospitals = _database.GetCollection<Hospital>("hospitals");
            _grupos = _database.GetCollection<GrupoPacienteHospital>("grupospacientehospital");
            _agendamentos = _database.GetCollection<Agendamento>("agendamentos");
            _medicalRecords = _database.GetCollection<RegistroMedico>("medicalrecords");
        }

        /// Ponto de entrada principal do script
        public async Task<UnificationResult> ExecuteAsync()
        {
            var result = new UnificationResult();

            try
            {
                _logger.LogInformation("[DESAFIO 3] Iniciando processo de unificação de pacientes duplicados");

                // 1. Identificar CPFs duplicados
                var duplicateCpfs = await IdentifyDuplicateCpfsAsync();
                result.TotalDuplicateCpfs = duplicateCpfs.Count;

                _logger.LogInformation($"Encontrados {duplicateCpfs.Count} CPFs com duplicatas");

                // 2. Processar cada CPF duplicado
                foreach (var cpf in duplicateCpfs)
                {
                    var cpfResult = await UnifyPatientsByCpfAsync(cpf);
                    result.ProcessedCpfs.Add(cpf);
                    result.TotalPatientsRemoved += cpfResult.PatientsRemoved;
                    result.TotalGroupsUpdated += cpfResult.GroupsUpdated;
                    result.TotalAppointmentsUpdated += cpfResult.AppointmentsUpdated;
                    result.TotalMedicalRecordsUpdated += cpfResult.MedicalRecordsUpdated;
                }

                // 3. Relatório final
                await GenerateReportAsync(result);

                _logger.LogInformation("[DESAFIO 3] Processo de unificação concluído com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o processo de unificação");
                result.Errors.Add($"Erro geral: {ex.Message}");
            }

            return result;
        }

        /// Identifica CPFs que possuem múltiplos pacientes cadastrados
        private async Task<List<string>> IdentifyDuplicateCpfsAsync()
        {
            var pipeline = new[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    ["_id"] = "$CPF",
                    ["count"] = new BsonDocument("$sum", 1)
                }),
                new BsonDocument("$match", new BsonDocument("count", new BsonDocument("$gt", 1))),
                new BsonDocument("$project", new BsonDocument("cpf", "$_id"))
            };

            var results = await _patients.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results.Select(doc => doc["cpf"].AsString).ToList();
        }

        /// Unifica pacientes duplicados para um CPF específico
        /// REGRA: Mantém o paciente com DataCadastro mais recente
        private async Task<CpfUnificationResult> UnifyPatientsByCpfAsync(string cpf)
        {
            var result = new CpfUnificationResult { CPF = cpf };

            try
            {
                // Buscar todos os pacientes com o mesmo CPF
                var duplicatePatients = await _patients
                    .Find(p => p.Cpf == cpf)
                    .ToListAsync();

                if (duplicatePatients.Count <= 1)
                {
                    _logger.LogWarning($"CPF {cpf} não possui duplicatas");
                    return result;
                }

                _logger.LogInformation($"Processando {duplicatePatients.Count} pacientes duplicados para CPF {cpf}");

                // Ordenar por DataCadastro (mais recente primeiro = mantido)
                var sortedPatients = duplicatePatients
                    .OrderByDescending(p => p.DataCadastro)
                    .ToList();

                var patientToKeep = sortedPatients.First();
                var patientsToRemove = sortedPatients.Skip(1).ToList();

                _logger.LogInformation($"Mantendo paciente: {patientToKeep.Id} ({patientToKeep.Nome}) - Cadastro: {patientToKeep.DataCadastro}");

                // Processar remoção de cada paciente duplicado
                foreach (var patientToRemove in patientsToRemove)
                {
                    _logger.LogInformation($"Removendo duplicata: {patientToRemove.Id} ({patientToRemove.Nome}) - Cadastro: {patientToRemove.DataCadastro}");

                    // Atualizar referências antes de remover
                    var subResult = await UpdateReferencesAndRemovePatientAsync(patientToRemove.Id, patientToKeep.Id);

                    result.PatientsRemoved++;
                    result.GroupsUpdated += subResult.GroupsUpdated;
                    result.AppointmentsUpdated += subResult.AppointmentsUpdated;
                    result.MedicalRecordsUpdated += subResult.MedicalRecordsUpdated;
                }

                // Limpar grupos duplicados para o paciente mantido
                await CleanupDuplicateGroupsAsync(patientToKeep.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar CPF {cpf}");
                result.Errors.Add($"CPF {cpf}: {ex.Message}");
            }

            return result;
        }

        /// Atualiza todas as referências do paciente a ser removido para o paciente mantido
        /// e remove o paciente duplicado
        private async Task<ReferenceUpdateResult> UpdateReferencesAndRemovePatientAsync(Guid patientToRemoveId, Guid patientToKeepId)
        {
            var result = new ReferenceUpdateResult();

            // 1. Atualizar GrupoPacienteHospital
            var gruposFilter = Builders<GrupoPacienteHospital>.Filter.Eq(g => g.PacienteId, patientToRemoveId);
            var gruposUpdate = Builders<GrupoPacienteHospital>.Update.Set(g => g.PacienteId, patientToKeepId);
            var gruposUpdateResult = await _grupos.UpdateManyAsync(gruposFilter, gruposUpdate);
            result.GroupsUpdated = (int)gruposUpdateResult.ModifiedCount;

            // 2. Atualizar Agendamentos
            var agendamentosFilter = Builders<Agendamento>.Filter.Eq(a => a.PacienteId, patientToRemoveId);
            var agendamentosUpdate = Builders<Agendamento>.Update.Set(a => a.PacienteId, patientToKeepId);
            var agendamentosUpdateResult = await _agendamentos.UpdateManyAsync(agendamentosFilter, agendamentosUpdate);
            result.AppointmentsUpdated = (int)agendamentosUpdateResult.ModifiedCount;

            // 3. Atualizar Registros Médicos
            var recordsFilter = Builders<RegistroMedico>.Filter.Eq(r => r.PacienteId, patientToRemoveId);
            var recordsUpdate = Builders<RegistroMedico>.Update.Set(r => r.PacienteId, patientToKeepId);
            var recordsUpdateResult = await _medicalRecords.UpdateManyAsync(recordsFilter, recordsUpdate);
            result.MedicalRecordsUpdated = (int)recordsUpdateResult.ModifiedCount;

            // 4. Remover paciente duplicado
            await _patients.DeleteOneAsync(p => p.Id == patientToRemoveId);

            _logger.LogInformation($"Referências atualizadas para paciente {patientToRemoveId} -> {patientToKeepId}: " +
                                 $"Grupos: {result.GroupsUpdated}, Agendamentos: {result.AppointmentsUpdated}, " +
                                 $"Registros Médicos: {result.MedicalRecordsUpdated}");

            return result;
        }

        /// Remove grupos duplicados para o mesmo paciente-hospital
        private async Task CleanupDuplicateGroupsAsync(Guid patientId)
        {
            var grupos = await _grupos.Find(g => g.PacienteId == patientId).ToListAsync();

            var duplicateGroups = grupos
                .GroupBy(g => new { g.PacienteId, g.HospitalId })
                .Where(group => group.Count() > 1)
                .ToList();

            foreach (var duplicateGroup in duplicateGroups)
            {
                var groupsToKeep = duplicateGroup.First();
                var groupsToRemove = duplicateGroup.Skip(1);

                foreach (var groupToRemove in groupsToRemove)
                {
                    await _grupos.DeleteOneAsync(g => g.Id == groupToRemove.Id);
                    _logger.LogInformation($"Removido grupo duplicado: {groupToRemove.Id}");
                }
            }
        }

        /// Gera relatório final do processo
        private async Task GenerateReportAsync(UnificationResult result)
        {
            _logger.LogInformation("=== RELATÓRIO FINAL DE UNIFICAÇÃO ===");
            _logger.LogInformation($"CPFs processados: {result.TotalDuplicateCpfs}");
            _logger.LogInformation($"Pacientes removidos: {result.TotalPatientsRemoved}");
            _logger.LogInformation($"Grupos atualizados: {result.TotalGroupsUpdated}");
            _logger.LogInformation($"Agendamentos atualizados: {result.TotalAppointmentsUpdated}");
            _logger.LogInformation($"Registros médicos atualizados: {result.TotalMedicalRecordsUpdated}");

            if (result.Errors.Any())
            {
                _logger.LogWarning($"Erros encontrados: {result.Errors.Count}");
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"  - {error}");
                }
            }

            // Verificar se ainda existem duplicatas
            var remainingDuplicates = await IdentifyDuplicateCpfsAsync();
            if (remainingDuplicates.Any())
            {
                _logger.LogWarning($"ATENÇÃO: Ainda existem {remainingDuplicates.Count} CPFs com duplicatas!");
                foreach (var cpf in remainingDuplicates)
                {
                    _logger.LogWarning($"  - CPF duplicado: {cpf}");
                }
            }
            else
            {
                _logger.LogInformation("✅ Nenhuma duplicata restante encontrada - Unificação bem-sucedida!");
            }

            _logger.LogInformation("========================================");
        }
    }

}
