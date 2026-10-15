using System.Text.RegularExpressions;
using FinX.Core.Entity;
using FinX.Core.Interface.Repository;
using FinX.Core.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinX.Data.Repository;

public sealed class PacienteRepository : IPacienteRepository
{
    private readonly IMongoCollection<Paciente> _pacientes;
    private readonly IMongoCollection<RegistroMedico> _registrosMedicos;

    public PacienteRepository(IMongoDatabase database)
    {
        _pacientes = database.GetCollection<Paciente>("patients");
        _registrosMedicos = database.GetCollection<RegistroMedico>("medicalrecords");
    }

    public async Task<Paciente> CriarAsync(Paciente paciente, CancellationToken cancellationToken = default)
    {
        paciente.Id = paciente.Id == Guid.Empty ? Guid.NewGuid() : paciente.Id;
        paciente.DataCadastro = paciente.DataCadastro == default ? DateTime.UtcNow : paciente.DataCadastro;
        await _pacientes.InsertOneAsync(paciente, cancellationToken: cancellationToken);
        return paciente;
    }

    public async Task<Paciente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _pacientes.Find(paciente => paciente.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<ResultadoPaginado<Paciente>> ConsultarAsync(FiltroPaciente filtro, CancellationToken cancellationToken = default)
    {
        var definicaoFiltro = CriarFiltro(filtro);
        var ordenacao = CriarOrdenacao(filtro);
        var salto = checked((filtro.Pagina - 1) * filtro.TamanhoPagina);

        var totalRegistrosTask = _pacientes.CountDocumentsAsync(definicaoFiltro, cancellationToken: cancellationToken);
        var itensTask = _pacientes.Find(definicaoFiltro).Sort(ordenacao).Skip(salto).Limit(filtro.TamanhoPagina).ToListAsync(cancellationToken);
        await Task.WhenAll(totalRegistrosTask, itensTask);

        return new ResultadoPaginado<Paciente>
        {
            Itens = itensTask.Result,
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalRegistros = totalRegistrosTask.Result
        };
    }

    public async Task<bool> AtualizarAsync(Paciente paciente, CancellationToken cancellationToken = default)
    {
        var resultado = await _pacientes.ReplaceOneAsync(existente => existente.Id == paciente.Id, paciente, cancellationToken: cancellationToken);
        return resultado.MatchedCount > 0;
    }

    public async Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var resultado = await _pacientes.DeleteOneAsync(paciente => paciente.Id == id, cancellationToken);
        if (resultado.DeletedCount > 0)
        {
            await _registrosMedicos.DeleteManyAsync(registro => registro.PacienteId == id, cancellationToken);
        }

        return resultado.DeletedCount > 0;
    }

    public async Task<RegistroMedico> AdicionarRegistroMedicoAsync(Guid pacienteId, RegistroMedico registro, CancellationToken cancellationToken = default)
    {
        registro.Id = Guid.NewGuid();
        registro.PacienteId = pacienteId;
        await _registrosMedicos.InsertOneAsync(registro, cancellationToken: cancellationToken);
        return registro;
    }

    public async Task<IReadOnlyList<RegistroMedico>> ObterHistoricoMedicoAsync(Guid pacienteId, CancellationToken cancellationToken = default) =>
        await _registrosMedicos.Find(registro => registro.PacienteId == pacienteId).SortByDescending(registro => registro.Data).ThenBy(registro => registro.Id).ToListAsync(cancellationToken);

    private static FilterDefinition<Paciente> CriarFiltro(FiltroPaciente filtro)
    {
        var filtros = new List<FilterDefinition<Paciente>>();

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            filtros.Add(Builders<Paciente>.Filter.Regex(paciente => paciente.Nome, new BsonRegularExpression(Regex.Escape(filtro.Nome.Trim()), "i")));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Cpf))
        {
            filtros.Add(Builders<Paciente>.Filter.Eq(paciente => paciente.Cpf, filtro.Cpf.Trim()));
        }

        if (filtro.NascimentoDe.HasValue)
        {
            filtros.Add(Builders<Paciente>.Filter.Gte(paciente => paciente.DataNascimento, filtro.NascimentoDe.Value.Date));
        }

        if (filtro.NascimentoAte.HasValue)
        {
            filtros.Add(Builders<Paciente>.Filter.Lt(paciente => paciente.DataNascimento, filtro.NascimentoAte.Value.Date.AddDays(1)));
        }

        return filtros.Count == 0 ? Builders<Paciente>.Filter.Empty : Builders<Paciente>.Filter.And(filtros);
    }

    private static SortDefinition<Paciente> CriarOrdenacao(FiltroPaciente filtro)
    {
        var ordenacao = filtro.OrdenarPor.ToLowerInvariant() switch
        {
            "cpf" => filtro.OrdemDescendente ? Builders<Paciente>.Sort.Descending(paciente => paciente.Cpf) : Builders<Paciente>.Sort.Ascending(paciente => paciente.Cpf),
            "datanascimento" => filtro.OrdemDescendente ? Builders<Paciente>.Sort.Descending(paciente => paciente.DataNascimento) : Builders<Paciente>.Sort.Ascending(paciente => paciente.DataNascimento),
            "datacadastro" => filtro.OrdemDescendente ? Builders<Paciente>.Sort.Descending(paciente => paciente.DataCadastro) : Builders<Paciente>.Sort.Ascending(paciente => paciente.DataCadastro),
            _ => filtro.OrdemDescendente ? Builders<Paciente>.Sort.Descending(paciente => paciente.Nome) : Builders<Paciente>.Sort.Ascending(paciente => paciente.Nome)
        };

        return ordenacao.Ascending(paciente => paciente.Id);
    }
}
