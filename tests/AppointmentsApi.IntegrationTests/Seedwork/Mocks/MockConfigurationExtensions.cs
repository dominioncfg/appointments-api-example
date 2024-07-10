using AppointmentsApi.IntegrationTests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentsApi.IntegrationTests.Seedwork;
public static class MockConfigurationExtensions
{
    public static IServiceCollection ConfigureMocks(this IServiceCollection services)
    {
        services.AddMockedHttpClient();
        return services;
    }


    public static TestServerFixture ResetMocks(this TestServerFixture fixture)
    {
        fixture.ResetMockedHttpClientMocks();
        return fixture;
    }

}