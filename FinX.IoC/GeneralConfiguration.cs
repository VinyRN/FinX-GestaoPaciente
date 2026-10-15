using FinX.Adapter;
using FinX.Core.Interface.Repository;
using FinX.Core.Interface.Service;
using FinX.Data;
using FinX.Data.Repository;
using FinX.Service;
using Microsoft.Extensions.DependencyInjection;

namespace FinX.IoC;

public static class GeneralConfiguration
{
    public static IServiceCollection AddGeneralConfiguration(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddGeneralRepositoryConfiguration();
        services.AddGeneralServiceConfiguration();
        services.AddGeneralAdapterConfiguration();
        return services;
    }

    private static IServiceCollection AddGeneralRepositoryConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IPacienteRepository, PacienteRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPacienteUnificacaoRepository, MongoPacienteUnificacaoRepository>();
        services.AddScoped<IGrupoPacienteHospitalRepository, MongoGrupoPacienteHospitalRepository>();
        services.AddScoped<IAgendamentoRepository, MongoAgendamentoRepository>();
        services.AddScoped<IHospitalRepository, MongoHospitalRepository>();
        return services;
    }

    private static IServiceCollection AddGeneralServiceConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IExamesExternosService, ExamesExternosSimuladosService>();
        services.AddScoped<IPacienteService, PacienteService>();
        services.AddScoped<IHospitalService, HospitalService>();
        services.AddScoped<IAgendamentoService, AgendamentoService>();
        services.AddScoped<IGrupoPacienteHospitalService, GrupoPacienteHospitalService>();
        services.AddScoped<IPacienteUnificacaoService, PacienteUnificacaoService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<ICriptografiaSenha, CriptografiaSenha>();
        return services;
    }

    private static IServiceCollection AddGeneralAdapterConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IKeyService, KeyService>();
        return services;
    }
}
