using System.Text.Json.Serialization;

namespace FinX.Core.Dto.Response.Agendamento;

public class AgendamentoResponseDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("hospitalId")]
    public Guid HospitalId { get; set; }
    [JsonPropertyName("pacienteId")]
    public Guid PacienteId { get; set; }
    [JsonPropertyName("data")]
    public DateTime Data { get; set; }
    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
