using FinX.Core.Dto.Request.Agendamento;
using FinX.Core.Dto.Response.Agendamento;
using FinX.Core.Entity;

namespace FinX.Api.Mapping;

public static class AgendamentoMapper
{
    public static Agendamento ToEntity(this AgendamentoRequestDto source) => new()
    {
        Id = source.Id,
        HospitalId = source.HospitalId,
        PacienteId = source.PacienteId,
        Data = source.Data,
        Descricao = source.Descricao,
        Status = source.Status
    };

    public static AgendamentoResponseDto ToResponse(this Agendamento source) => new()
    {
        Id = source.Id,
        HospitalId = source.HospitalId,
        PacienteId = source.PacienteId,
        Data = source.Data,
        Descricao = source.Descricao,
        Status = source.Status
    };
}
