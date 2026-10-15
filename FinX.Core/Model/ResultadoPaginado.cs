namespace FinX.Core.Model;

public sealed class ResultadoPaginado<T>
{
    public IReadOnlyList<T> Itens { get; init; } = [];
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
    public long TotalRegistros { get; init; }
    public int TotalPaginas => TamanhoPagina == 0
        ? 0
        : (int)Math.Ceiling(TotalRegistros / (double)TamanhoPagina);
}
