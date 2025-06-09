using MotoHub.Application.Interfaces.UseCases.Couriers;
using MotoHub.Application.Interfaces.UseCases.Motorcycles;
using MotoHub.Application.Interfaces.UseCases.Renting;
using MotoHub.Application.Services;
using MotoHub.Application.UseCases.Couriers;
using MotoHub.Application.UseCases.Motorcycles;
using MotoHub.Application.UseCases.Renting;
using MotoHub.Domain.Interfaces;
using MotoHub.Domain.ValueObjects;

namespace MotoHub.API.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddUseCases();

        services.AddSingleton<IRentPricingCalculator, DefaultRentPricingCalculator>();

        List<RentPlan> rentPlans = configuration.GetRequiredSection("RentPlanCatalog")
                                                .Get<List<RentPlan>>()!;

        services.AddSingleton<IRentPlanCatalog, RentPlanCatalog>(_ => new RentPlanCatalog(rentPlans));

        return services;
    }

    private static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        // Motorcycle
        services.AddScoped<ISearchMotorcyclesUseCase, SearchMotorcyclesUseCase>();
        services.AddScoped<IRegisterMotorcycleUseCase, RegisterMotorcycleUseCase>();
        services.AddScoped<IUpdateMotorcycleUseCase, UpdateMotorcycleUseCase>();
        services.AddScoped<IGetMotorcycleByIdentifierUseCase, GetMotorcycleByIdentifierUseCase>();
        services.AddScoped<IDeleteMotorcycleUseCase, DeleteMotorcycleUseCase>();

        // Courier
        services.AddScoped<IRegisterCourierUseCase, RegisterCourierUseCase>();
        services.AddScoped<IUpdateCourierUseCase, UpdateCourierUseCase>();

        // Renting
        services.AddScoped<IRentMotorcycleUseCase, RentMotorcycleUseCase>();
        services.AddScoped<IGetRentByIdentifierUseCase, GetRentByIdentifierUseCase>();
        services.AddScoped<IReturnMotorcycleUseCase, ReturnMotorcycleUseCase>();

        return services;
    }
}