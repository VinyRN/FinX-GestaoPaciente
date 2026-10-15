using System.Text.Json.Serialization;

namespace FinX.Core.Dto.Request.Hospital;

public class HospitalRequestDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;
    [JsonPropertyName("cnpj")]
    public string Cnpj { get; set; } = string.Empty;
}
