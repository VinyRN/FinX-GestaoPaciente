using FinX.Core.Dto.Response.Exame;
namespace FinX.Core.Interface.Service;

public interface IExamesExternosService
{
    ExamesExternosResponseDto ConsultarPorCpf(string cpf);
}
