using System.Text.Json.Serialization;

namespace FinX.Core.Dto.Request.Paciente;

public class PacienteRequestDto
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
