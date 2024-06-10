using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Reflection;

namespace Ordering.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        //services.AddCarter();

        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        //app.MapCarter();

        return app;
    }
}