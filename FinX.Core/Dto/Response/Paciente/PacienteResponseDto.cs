using System.Text.Json.Serialization;

namespace FinX.Core.Dto.Response.Paciente;

public class PacienteResponseDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("name")]
    public string Nome { get; set; } = string.Empty;
    [JsonPropertyName("cpf")]
    public string Cpf { get; set; } = string.Empty;
    [JsonPropertyName("dateOfBirth")]
    public DateTime DataNascimento { get; set; }
    [JsonPropertyName("contact")]
    public string Contato { get; set; } = string.Empty;
    [JsonPropertyName("dataCadastro")]
    public DateTime DataCadastro { get; set; }
}
