using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MotoHub.Application.Interfaces;
using MotoHub.Application.Interfaces.Messaging;
using MotoHub.Infrastructure.Messaging;
using MotoHub.Infrastructure.Persistence;

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

        services.AddRepositories();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRentRepository, RentRepository>();

        return services;
    }

    public static void EnsureDatabaseCreated(this IHost app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
    }
}