using AppointmentsApi.Application;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentsApi.Api;

internal static class FluentValidationConfiguration
{
    public static IServiceCollection AddCustomFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(ApplicationConfiguration.Assembly);
        return services;
    }
}
