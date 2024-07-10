using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentsApi.Api;

public static class AppointmentsApiExtension
{
    public static IServiceCollection ConfigureAppointmentsApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddApplicationPart(typeof(AppointmentsApiExtension).Assembly);
        services
            .AddCustomMediatR()
            .AddCustomFluentValidation()
            .AddCustomProblemDetails()
            .AddAppointmentsInfrastructure(configuration);
        return services;
    }

    public static IApplicationBuilder UseAppointmentsApi(this IApplicationBuilder application)
    {
        application
            .UseCustomProblemDetails()
            .UseRouting()
            .UseEndpoints(endpoints =>
             {
                 endpoints.MapDefaultControllerRoute();
             });
        return application;
    }
}

