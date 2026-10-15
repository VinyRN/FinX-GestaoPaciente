namespace FinX.Core.Dto.Response.Paciente;

public class RegistroMedicoResponseDto
{
    public Guid Id { get; set; }
    public Guid PacienteId { get; set; }
    public DateTime Data { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}
