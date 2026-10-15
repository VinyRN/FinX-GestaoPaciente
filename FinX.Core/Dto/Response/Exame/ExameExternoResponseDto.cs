namespace FinX.Core.Dto.Response.Exame;

public class ExameExternoResponseDto
{
    public Guid Id { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public ExameResultadoResponseDto Result { get; set; } = new();
}
