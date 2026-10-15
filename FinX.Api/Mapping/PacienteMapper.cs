using FinX.Core.Dto.Request.Paciente;
using FinX.Core.Dto.Response.Comum;
using FinX.Core.Dto.Response.Paciente;
using FinX.Core.Entity;
using FinX.Core.Model;

namespace FinX.Api.Mapping;

public static class PacienteMapper
{
    public static Paciente ToEntity(this PacienteRequestDto source) => new()
    {
        Id = source.Id,
        Nome = source.Nome.Trim(),
        Cpf = source.Cpf.Trim(),
        DataNascimento = source.DataNascimento,
        Contato = source.Contato.Trim(),
        DataCadastro = source.DataCadastro
    };

    public static PacienteResponseDto ToResponse(this Paciente source) => new()
    {
        Id = source.Id,
        Nome = source.Nome,
        Cpf = source.Cpf,
        DataNascimento = source.DataNascimento,
        Contato = source.Contato,
        DataCadastro = source.DataCadastro
    };

    public static FiltroPaciente ToFiltro(this PacienteFiltroRequestDto source) => new()
    {
        Nome = source.Nome,
        Cpf = source.Cpf,
        NascimentoDe = source.NascimentoDe,
        NascimentoAte = source.NascimentoAte,
        Pagina = source.Pagina,
        TamanhoPagina = source.TamanhoPagina,
        OrdenarPor = source.OrdenarPor,
        OrdemDescendente = source.Direcao.Equals("desc", StringComparison.OrdinalIgnoreCase)
    };

    public static RegistroMedicoResponseDto ToResponse(this RegistroMedico source) => new()
    {
        Id = source.Id,
        PacienteId = source.PacienteId,
        Data = source.Data,
        Tipo = source.Tipo,
        Descricao = source.Descricao
    };

    public static PaginacaoResponseDto<TDestino> ToResponse<TOrigem, TDestino>(
        this ResultadoPaginado<TOrigem> source,
        Func<TOrigem, TDestino> mapear) => new()
        {
            Itens = source.Itens.Select(mapear).ToList(),
            Pagina = source.Pagina,
            TamanhoPagina = source.TamanhoPagina,
            TotalRegistros = source.TotalRegistros,
            TotalPaginas = source.TotalPaginas
        };
}
