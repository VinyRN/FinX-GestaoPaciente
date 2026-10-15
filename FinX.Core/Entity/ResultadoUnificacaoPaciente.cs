using System.Text.Json.Serialization;

namespace FinX.Core.Entity;

public sealed class ResultadoUnificacaoPaciente
{
    [JsonPropertyName("totalDuplicatesFound")]
    public int TotalDuplicadosEncontrados { get; set; }

    [JsonPropertyName("patientsRemoved")]
    public int PacientesRemovidos { get; set; }

    [JsonPropertyName("patientsKept")]
    public int PacientesMantidos { get; set; }

    [JsonPropertyName("groupsUpdated")]
    public int GruposAtualizados { get; set; }

    [JsonPropertyName("appointmentsUpdated")]
    public int AgendamentosAtualizados { get; set; }

    [JsonPropertyName("processedCpfs")]
    public List<string> CpfsProcessados { get; set; } = [];

    [JsonPropertyName("errors")]
    public List<string> Erros { get; set; } = [];
}
