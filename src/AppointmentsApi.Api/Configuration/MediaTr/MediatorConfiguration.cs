using AppointmentsApi.Application;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentsApi.Api;


internal static class MediatorConfiguration
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services
           .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ApplicationConfiguration.Assembly))
           .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        
        return services;
    }
}