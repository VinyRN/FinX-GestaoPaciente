using FinX.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace FinX.Tests;

public sealed class PacienteControllerRouteTests
{
    [Fact]
    public void PacienteController_DeveExporSomenteRotaPadrao()
    {
        var rota = typeof(PacienteController)
            .GetCustomAttributes(typeof(RouteAttribute), false)
            .Cast<RouteAttribute>()
            .Single();

        Assert.Equal("api/Paciente", rota.Template);
    }
}
