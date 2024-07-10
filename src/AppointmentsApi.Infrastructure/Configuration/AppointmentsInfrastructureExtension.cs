using AppointmentsApi.Domain.Services;
using AppointmentsApi.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;

namespace AppointmentsApi.Api;

public static class AppointmentsInfrastructureExtension
{

    public static IServiceCollection AddAppointmentsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var apiConfiguration = new AppointmentsApiConfiguration();
        configuration.GetSection(AppointmentsApiConfiguration.SectionName).Bind(apiConfiguration);

        services.AddScoped<IAppointmentsApiClient, AppointmentsApiClient>();
        services.AddHttpClient<IAppointmentsApiClient, AppointmentsApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiConfiguration.BaseUrl);
            var authToken = Encoding.ASCII.GetBytes($"{apiConfiguration.UserName}:{apiConfiguration.Password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

        });
        return services;
    }

}

