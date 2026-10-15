using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FinX.IoC;

public static class MongoConfiguration
{
    public static IServiceCollection AddMongoConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["MONGODB_URI"] ?? configuration["Mongo:ConnectionString"];
        var databaseName = configuration["MONGODB_DATABASE"] ?? configuration["Mongo:Database"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Configure MONGODB_URI ou Mongo:ConnectionString.");
        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException("Configure MONGODB_DATABASE ou Mongo:Database.");

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
        return services;
    }
}
