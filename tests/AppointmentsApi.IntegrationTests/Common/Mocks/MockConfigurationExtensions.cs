using AppointmentsApi.Domain.Services;
using AppointmentsApi.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using System.Net.Http.Headers;
using System.Text;

namespace AppointmentsApi.IntegrationTests.Common;

public static class MockedAppointmentClientConfiguration
{
    public const string BaseUrl = "http://testservice.com";
    public const string UserName = "testUser";
    public const string Password = "testPass";
}

public static class MockConfigurationExtensions
{
    public static IServiceCollection AddMockedHttpClient(this IServiceCollection services)
    {
        var mockHttpHandler = new MockHttpMessageHandler();

        services.AddSingleton(mockHttpHandler);

        services.AddScoped<IAppointmentsApiClient, AppointmentsApiClient>();
        services.AddHttpClient<IAppointmentsApiClient, AppointmentsApiClient>(_ =>
        {
            var mockedClient = new HttpClient(mockHttpHandler);
            mockedClient.BaseAddress = new Uri(MockedAppointmentClientConfiguration.BaseUrl);
            var authToken = Encoding.ASCII.GetBytes($"{MockedAppointmentClientConfiguration.UserName}:{MockedAppointmentClientConfiguration.Password}");
            mockedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            return new AppointmentsApiClient(mockedClient);
        });
        return services;
    }

    public static TestServerFixture ResetMockedHttpClientMocks(this TestServerFixture fixture)
    {
        fixture.ExecuteScope(serviceProvider =>
        {
            var handler = serviceProvider.GetRequiredService<MockHttpMessageHandler>();
            handler.ResetBackendDefinitions();
            handler.ResetExpectations();
        });
        return fixture;
    }
}