namespace FinX.Core.Model;

public sealed class FiltroPaciente
{
    public string? Nome { get; init; }
    public string? Cpf { get; init; }
    public DateTime? NascimentoDe { get; init; }
    public DateTime? NascimentoAte { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string OrdenarPor { get; init; } = "nome";
    public bool OrdemDescendente { get; init; }
}
