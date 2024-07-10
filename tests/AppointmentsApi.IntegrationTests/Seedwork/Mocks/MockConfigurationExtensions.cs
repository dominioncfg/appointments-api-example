using AppointmentsApi.Domain.Services;
using AppointmentsApi.IntegrationTests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentsApi.IntegrationTests.Seedwork;
public static class MockConfigurationExtensions
{
    public static IServiceCollection ConfigureMocks(this IServiceCollection services)
    {
        var mockedApiClient = new MockedAppointmentsApiClient();
        services.AddSingleton(mockedApiClient);
        services.AddSingleton<IAppointmentsApiClient, MockedAppointmentsApiClient>(_ => mockedApiClient);
        return services;
    }


    public static TestServerFixture ResetMocks(this TestServerFixture fixture)
    {
        MockedAppointmentsApiClient.ClearStorage();
        return fixture;
    }

}