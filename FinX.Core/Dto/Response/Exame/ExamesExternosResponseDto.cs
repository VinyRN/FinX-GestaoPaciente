namespace FinX.Core.Dto.Response.Exame;

public class ExamesExternosResponseDto
{
    public string Cpf { get; set; } = string.Empty;
    public List<ExameExternoResponseDto> Exams { get; set; } = new();
}
