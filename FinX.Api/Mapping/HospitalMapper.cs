using FinX.Core.Dto.Request.Hospital;
using FinX.Core.Dto.Response.Hospital;
using FinX.Core.Entity;

namespace FinX.Api.Mapping;

public static class HospitalMapper
{
    public static Hospital ToEntity(this HospitalRequestDto source) => new()
    {
        Id = source.Id,
        Nome = source.Nome,
        Cnpj = source.Cnpj
    };

    public static HospitalResponseDto ToResponse(this Hospital source) => new()
    {
        Id = source.Id,
        Nome = source.Nome,
        Cnpj = source.Cnpj
    };
}
