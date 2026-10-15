using System.ComponentModel.DataAnnotations;

namespace FinX.Core.Dto.Request.Paciente;

public sealed class PacienteFiltroRequestDto
{
    public string? Nome { get; set; }
    public string? Cpf { get; set; }
    public DateTime? NascimentoDe { get; set; }
    public DateTime? NascimentoAte { get; set; }

    [Range(1, int.MaxValue)]
    public int Pagina { get; set; } = 1;

    [Range(1, 100)]
    public int TamanhoPagina { get; set; } = 20;

    [RegularExpression("^(nome|cpf|dataNascimento|dataCadastro)$")]
    public string OrdenarPor { get; set; } = "nome";

    [RegularExpression("^(asc|desc)$")]
    public string Direcao { get; set; } = "asc";
}
