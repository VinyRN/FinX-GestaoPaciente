using System.Text.Json.Serialization;
namespace FinX.Core.Dto.Response.Exame;

public class ExameResultadoResponseDto
{
    public string Summary { get; set; } = string.Empty;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Values { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ReportUrl { get; set; }
}
