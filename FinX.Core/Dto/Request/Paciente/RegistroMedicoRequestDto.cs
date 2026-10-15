namespace FinX.Core.Dto.Request.Paciente;

public class RegistroMedicoRequestDto
{
    public DateTime Data { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}
