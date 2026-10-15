using FinX.Core.Entity;
using FinX.Core.Model;
using FinX.Service;
using FinX.Tests.Fakes;
using Xunit;

namespace FinX.Tests;

public sealed class PacienteServiceTests
{
    [Fact]
    public async Task ConsultarAsync_DeveRetornarPaginaComTotalDeRegistrosETotalDePaginas()
    {
        var pacientes = new[]
        {
            CriarPaciente("Ana", "11111111111"),
            CriarPaciente("Beatriz", "22222222222"),
            CriarPaciente("Carlos", "33333333333")
        };
        var service = new PacienteService(new FakePacienteRepository(pacientes));

        var resultado = await service.ConsultarAsync(new FiltroPaciente { Pagina = 2, TamanhoPagina = 2 });

        Assert.Single(resultado.Itens);
        Assert.Equal("Carlos", resultado.Itens[0].Nome);
        Assert.Equal(3, resultado.TotalRegistros);
        Assert.Equal(2, resultado.TotalPaginas);
    }

    [Fact]
    public async Task ConsultarAsync_DeveAplicarFiltroPorCpf()
    {
        var esperado = CriarPaciente("Ana", "11111111111");
        var service = new PacienteService(new FakePacienteRepository(new[]
        {
            esperado,
            CriarPaciente("Beatriz", "22222222222")
        }));

        var resultado = await service.ConsultarAsync(new FiltroPaciente { Cpf = esperado.Cpf });

        Assert.Single(resultado.Itens);
        Assert.Equal(esperado.Id, resultado.Itens[0].Id);
        Assert.Equal(1, resultado.TotalRegistros);
    }

    private static Paciente CriarPaciente(string nome, string cpf) => new()
    {
        Id = Guid.NewGuid(),
        Nome = nome,
        Cpf = cpf,
        DataNascimento = new DateTime(1990, 1, 1),
        Contato = $"{nome.ToLowerInvariant()}@email.com",
        DataCadastro = DateTime.UtcNow
    };
}
