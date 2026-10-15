using System.Text.Json.Serialization;

namespace FinX.Core.Dto.Response.Hospital;

public class HospitalResponseDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;
    [JsonPropertyName("cnpj")]
    public string Cnpj { get; set; } = string.Empty;
}
