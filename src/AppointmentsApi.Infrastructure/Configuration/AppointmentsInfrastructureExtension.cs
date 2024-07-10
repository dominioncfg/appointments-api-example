using AppointmentsApi.Domain.Services;
using AppointmentsApi.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;

namespace AppointmentsApi.Api;

public static class AppointmentsInfrastructureExtension
{

    public static IServiceCollection AddAppointmentsInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IAppointmentsApiClient, AppointmentsApiClient>();
        services.AddHttpClient<IAppointmentsApiClient, AppointmentsApiClient>(client =>
        {
            client.BaseAddress = new Uri(AppointmentsApiConfiguration.ApiBaseUrl);
            var authToken = Encoding.ASCII.GetBytes($"{AppointmentsApiConfiguration.UserName}:{AppointmentsApiConfiguration.Password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

        });
        return services;
    }

}

