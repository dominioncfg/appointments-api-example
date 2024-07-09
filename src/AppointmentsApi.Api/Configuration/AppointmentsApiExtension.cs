using AppointmentsApi.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentsApi.Api;

public static class AppointmentsApiExtension
{
    public static IServiceCollection ConfigureAppointmentsApiServices(this IServiceCollection services)
    {
        services.AddControllers()
            .AddApplicationPart(typeof(AppointmentsApiExtension).Assembly);
        services
            .AddCustomMediatR()
            .AddCustomFluentValidation()
            .AddCustomProblemDetails();
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

