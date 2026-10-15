using FinX.Core.Interface.Repository;
using FinX.Core.Interface.Service;
using FinX.Data.Repository;
using FinX.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace FinX.Scripts
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var command = args.Length > 0 ? args[0].ToLower() : "help";

            Console.WriteLine("=== DESAFIO 3: Scripts de Unificação de Pacientes Duplicados ===");
            Console.WriteLine($"Comando executado: {command}");
            Console.WriteLine($"Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine("");

            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("=== DESAFIO 3: Scripts de Unificação de Pacientes Duplicados ===");
            logger.LogInformation($"Comando executado: {command}");
            logger.LogInformation($"Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            logger.LogInformation("");

            try
            {
                switch (command)
                {
                    case "create-test-data":
                        await ExecuteCreateTestDataAsync(host.Services, logger);
                        break;

                    case "unify-duplicates":
                        await ExecuteUnifyDuplicatesAsync(host.Services, logger);
                        break;

                    case "full-demo":
                        await ExecuteFullDemoAsync(host.Services, logger);
                        break;

                    case "create-user":
                        await ExecuteCreateUserAsync(host.Services, logger);
                        break;

                    case "check-connection":
                        await ExecuteCheckConnectionAsync(host.Services, logger);
                        break;

                    case "help":
                    default:
                        ShowHelp(logger);
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro durante execução do script");
                Environment.Exit(1);
            }

            logger.LogInformation("");
            logger.LogInformation("=== Execução finalizada ===");
        }

        private static async Task ExecuteCreateTestDataAsync(IServiceProvider services, ILogger logger)
        {
            logger.LogInformation("🎯 Executando: Criação de dados de teste");

            var database = services.GetRequiredService<IMongoDatabase>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            var script = new CreateTestData(database, loggerFactory.CreateLogger<CreateTestData>());
            var result = await script.ExecuteAsync(clearExistingData: true);

            logger.LogInformation("✅ Dados de teste criados:");
            logger.LogInformation($"   - Hospitais: {result.HospitalsCreated}");
            logger.LogInformation($"   - Pacientes (c/ duplicatas): {result.PatientsCreated}");
            logger.LogInformation($"   - Vínculos: {result.GroupsCreated}");
            logger.LogInformation($"   - Agendamentos: {result.AppointmentsCreated}");

            if (result.Errors.Count > 0)
            {
                logger.LogWarning($"⚠️ Erros encontrados: {result.Errors.Count}");
                result.Errors.ForEach(error => logger.LogError($"   - {error}"));
            }
        }

        private static async Task ExecuteUnifyDuplicatesAsync(IServiceProvider services, ILogger logger)
        {
            logger.LogInformation("🔄 Executando: Unificação de pacientes duplicados");

            var database = services.GetRequiredService<IMongoDatabase>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            var script = new UnifyDuplicatePatients(database, loggerFactory.CreateLogger<UnifyDuplicatePatients>());
            var result = await script.ExecuteAsync();

            logger.LogInformation("✅ Unificação concluída:");
            logger.LogInformation($"   - CPFs processados: {result.TotalDuplicateCpfs}");
            logger.LogInformation($"   - Pacientes removidos: {result.TotalPatientsRemoved}");
            logger.LogInformation($"   - Grupos atualizados: {result.TotalGroupsUpdated}");
            logger.LogInformation($"   - Agendamentos atualizados: {result.TotalAppointmentsUpdated}");
            logger.LogInformation($"   - Registros médicos: {result.TotalMedicalRecordsUpdated}");

            if (result.Errors.Count > 0)
            {
                logger.LogWarning($"⚠️ Erros encontrados: {result.Errors.Count}");
                result.Errors.ForEach(error => logger.LogError($"   - {error}"));
            }
        }

        private static async Task ExecuteFullDemoAsync(IServiceProvider services, ILogger logger)
        {
            logger.LogInformation("🚀 Executando: Demonstração completa do DESAFIO 3");
            logger.LogInformation("");

            logger.LogInformation("Etapa 1/2: Criando dados de teste...");
            await ExecuteCreateTestDataAsync(services, logger);

            logger.LogInformation("");

            // Etapa 2: Unificar duplicatas
            logger.LogInformation("Etapa 2/2: Unificando pacientes duplicados...");
            await ExecuteUnifyDuplicatesAsync(services, logger);

            logger.LogInformation("");
            logger.LogInformation("🎉 Demonstração completa finalizada com sucesso!");
            logger.LogInformation("💡 Regra aplicada: Mantido o paciente com DataCadastro mais recente");
            logger.LogInformation("💡 Todas as referências foram atualizadas para o paciente mantido");
        }

        private static async Task ExecuteCreateUserAsync(IServiceProvider services, ILogger logger)
        {
            var nomeUsuario = Environment.GetEnvironmentVariable("FINX_ADMIN_USERNAME") ?? "admin";
            var senha = Environment.GetEnvironmentVariable("FINX_ADMIN_PASSWORD");
            var perfil = Environment.GetEnvironmentVariable("FINX_ADMIN_ROLE") ?? "admin";
            var configuration = services.GetRequiredService<IConfiguration>();
            var connectionString = configuration["MONGODB_URI"]
                                   ?? configuration.GetConnectionString("MongoDB")
                                   ?? string.Empty;

            if (string.IsNullOrWhiteSpace(senha))
            {
                throw new InvalidOperationException("Configure FINX_ADMIN_PASSWORD com pelo menos 12 caracteres.");
            }

            if (connectionString.Contains("localhost", StringComparison.OrdinalIgnoreCase)
                || connectionString.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("A criação de usuário recusou o MongoDB local. Configure MONGODB_URI para o Atlas.");
            }

            var criador = services.GetRequiredService<CriadorUsuario>();
            var usuario = await criador.ExecutarAsync(nomeUsuario, senha, perfil);
            var database = services.GetRequiredService<IMongoDatabase>();
            logger.LogInformation(
                "Usuário {NomeUsuario} criado no banco {Banco} com o perfil {Perfil}.",
                usuario.NomeUsuario,
                database.DatabaseNamespace.DatabaseName,
                usuario.Perfil);
        }

        private static async Task ExecuteCheckConnectionAsync(IServiceProvider services, ILogger logger)
        {
            var database = services.GetRequiredService<IMongoDatabase>();
            await database.RunCommandAsync<MongoDB.Bson.BsonDocument>(new MongoDB.Bson.BsonDocument("ping", 1));
            logger.LogInformation("Conexão confirmada com o banco {Banco}.", database.DatabaseNamespace.DatabaseName);
        }

        private static void ShowHelp(ILogger logger)
        {
            Console.WriteLine("📖 DESAFIO 3 - Scripts de Unificação de Pacientes Duplicados");
            Console.WriteLine("");
            Console.WriteLine("Comandos disponíveis:");
            Console.WriteLine("");
            Console.WriteLine("  create-test-data   - Cria dados de teste com pacientes duplicados");
            Console.WriteLine("  unify-duplicates   - Executa unificação de pacientes duplicados");
            Console.WriteLine("  full-demo          - Demonstração completa (criar + unificar)");
            Console.WriteLine("  create-user        - Cria o primeiro usuário para autenticação");
            Console.WriteLine("  check-connection   - Valida a conexão configurada com o MongoDB");
            Console.WriteLine("  help               - Exibe esta ajuda");
            Console.WriteLine("");
            Console.WriteLine("Exemplos de uso:");
            Console.WriteLine("");
            Console.WriteLine("  dotnet run create-test-data");
            Console.WriteLine("  dotnet run unify-duplicates");
            Console.WriteLine("  dotnet run full-demo");
            Console.WriteLine("  dotnet run create-user");
            Console.WriteLine("  dotnet run check-connection");
            Console.WriteLine("");
            Console.WriteLine("Regra de Unificação:");
            Console.WriteLine("  • Pacientes com mesmo CPF são considerados duplicatas");
            Console.WriteLine("  • É mantido o paciente com DataCadastro mais recente");
            Console.WriteLine("  • Todas as referências são atualizadas (grupos, agendamentos, registros médicos)");
            Console.WriteLine("  • Pacientes duplicados são removidos após atualização das referências");

            logger.LogInformation("📖 DESAFIO 3 - Scripts de Unificação de Pacientes Duplicados");
            logger.LogInformation("");
            logger.LogInformation("Comandos disponíveis:");
            logger.LogInformation("");
            logger.LogInformation("  create-test-data   - Cria dados de teste com pacientes duplicados");
            logger.LogInformation("  unify-duplicates   - Executa unificação de pacientes duplicados");
            logger.LogInformation("  full-demo          - Demonstração completa (criar + unificar)");
            logger.LogInformation("  create-user        - Cria o primeiro usuário para autenticação");
            logger.LogInformation("  check-connection   - Valida a conexão configurada com o MongoDB");
            logger.LogInformation("  help               - Exibe esta ajuda");
            logger.LogInformation("");
            logger.LogInformation("Exemplos de uso:");
            logger.LogInformation("");
            logger.LogInformation("  dotnet run create-test-data");
            logger.LogInformation("  dotnet run unify-duplicates");
            logger.LogInformation("  dotnet run full-demo");
            logger.LogInformation("  dotnet run create-user");
            logger.LogInformation("  dotnet run check-connection");
            logger.LogInformation("");
            logger.LogInformation("Regra de Unificação:");
            logger.LogInformation("  • Pacientes com mesmo CPF são considerados duplicatas");
            logger.LogInformation("  • É mantido o paciente com DataCadastro mais recente");
            logger.LogInformation("  • Todas as referências são atualizadas (grupos, agendamentos, registros médicos)");
            logger.LogInformation("  • Pacientes duplicados são removidos após atualização das referências");
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory)
                          .AddJsonFile("appsettings.json", optional: false)
                          .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true)
                          .AddUserSecrets<Program>(optional: true)
                          .AddEnvironmentVariables()
                          .AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    var mongoConnectionString = configuration["MONGODB_URI"]
                                               ?? configuration.GetConnectionString("MongoDB")
                                               ?? "mongodb://localhost:27017";
                    var mongoDatabaseName = configuration["MONGODB_DATABASE"]
                                            ?? configuration["MongoDB:Database"]
                                            ?? "finxdb";

                    var mongoClient = new MongoClient(mongoConnectionString);
                    var database = mongoClient.GetDatabase(mongoDatabaseName);

                    services.AddSingleton<IMongoClient>(mongoClient);
                    services.AddSingleton(database);
                    services.AddScoped<IUsuarioRepository, UsuarioRepository>();
                    services.AddSingleton<ICriptografiaSenha, CriptografiaSenha>();
                    services.AddScoped<CriadorUsuario>();

                    services.AddLogging(builder =>
                    {
                        builder.AddSimpleConsole(options =>
                        {
                            options.IncludeScopes = false;
                            options.TimestampFormat = "HH:mm:ss ";
                        });
                    });
                });
    }
}
