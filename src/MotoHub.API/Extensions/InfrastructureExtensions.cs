using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MotoHub.Application.Interfaces.Messaging;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Infrastructure.Messaging;
using MotoHub.Infrastructure.Persistence;
using MotoHub.Infrastructure.Repositories;

namespace MotoHub.API.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(static options => options.UseSqlite("Data Source=motohub.db"));

        services.Configure<AwsSQSSettings>(configuration.GetSection("AwsSQSSettings"));

        services.AddSingleton<IAmazonSQS>(sp =>
        {
            AmazonSQSConfig config = new()
            {
                RegionEndpoint = RegionEndpoint.USEast1,
            };

            IOptions<AwsSQSSettings> options = sp.GetRequiredService<IOptions<AwsSQSSettings>>();

            AWSCredentials credentials = new BasicAWSCredentials(options.Value.Key, options.Value.Secret);

            return new AmazonSQSClient(credentials, config);
        });

        services.AddSingleton<IMotorcycleEventPublisher, AwsSQSMotorcycleEventPublisher>();

        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        services.AddSingleton(sp =>
        {
            MongoDbSettings mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;

            MongoClientSettings clientSettings = MongoClientSettings.FromConnectionString(mongoDbSettings.ConnectionString);

            MongoClient mongoClient = new(clientSettings);

            return mongoClient.GetDatabase(mongoDbSettings.Database);
        });

        services.AddRepositories();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRentRepository, RentRepository>();
        services.AddScoped<IImageRepository, MongoDbImageRepository>();

        return services;
    }

    public static void EnsureDatabaseCreated(this IHost app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
    }
}