using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MotoHub.Application.Interfaces;
using MotoHub.Application.Interfaces.Messaging;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Infrastructure.ImageStorage;
using MotoHub.Infrastructure.Messaging;
using MotoHub.Infrastructure.Persistence;
using MotoHub.Infrastructure.Repositories;
using MotoHub.Infrastructure.Settings;

namespace MotoHub.API.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(static options => options.UseSqlite("Data Source=motohub.db"));
        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRentRepository, RentRepository>();

        services.UseSQSAsMessageQueue(configuration);

        bool useS3AsImageStorage = configuration.GetSection("UseS3AsImageStorage")
                                                .Get<bool>();

        if (useS3AsImageStorage)
        {
            services.UseS3AsImageStorage(configuration);
        }
        else
        {
            services.UseMongoAsImageStorage(configuration);
        }

        return services;
    }

    private static IServiceCollection UseSQSAsMessageQueue(this IServiceCollection services, IConfiguration configuration)
    {
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

        return services;
    }

    private static IServiceCollection UseMongoAsImageStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        services.AddSingleton(sp =>
        {
            MongoDbSettings mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;

            MongoClientSettings clientSettings = MongoClientSettings.FromConnectionString(mongoDbSettings.ConnectionString);

            MongoClient mongoClient = new(clientSettings);

            return mongoClient.GetDatabase(mongoDbSettings.Database);
        });

        services.AddScoped<IImageStorage, MongoDbImageStorage>();

        return services;
    }

 

    private static IServiceCollection UseS3AsImageStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AwsS3Settings>(configuration.GetSection("AwsS3Settings"));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            AmazonS3Config amazonS3Config = new()
            {
                RegionEndpoint = RegionEndpoint.USEast1,
            };

            AwsS3Settings awsS3Settings = sp.GetRequiredService<IOptions<AwsS3Settings>>().Value;

            AWSCredentials credentials = new BasicAWSCredentials(awsS3Settings.Key, awsS3Settings.Secret);

            return new AmazonS3Client(credentials, amazonS3Config);
        });

        services.AddScoped<IImageStorage, AwsS3ImageStorage>();

        return services;
    }

    public static void EnsureDatabaseCreated(this IHost app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
    }
}