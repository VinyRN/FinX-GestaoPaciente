namespace FinX.Core.Dto.Response.Comum;

public sealed class PaginacaoResponseDto<T>
{
    public IReadOnlyList<T> Itens { get; init; } = [];
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
    public long TotalRegistros { get; init; }
    public int TotalPaginas { get; init; }
}
