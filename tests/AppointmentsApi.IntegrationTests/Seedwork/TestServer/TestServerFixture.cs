using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using AppointmentsApi.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;


namespace AppointmentsApi.IntegrationTests.Seedwork;

public sealed class TestServerFixture : IDisposable
{
    public TestServer Server { get; }
    private static TestServerFixture? FixtureInstance { get; set; }

    public static void OnTestInitResetApplicationState()
    {
        FixtureInstance!.OnTestInitResetApplicationServices();
    }

    public TestServerFixture()
    {
        IHostBuilder hostBuilder = ConfigureHost();
        var host = hostBuilder.StartAsync().GetAwaiter().GetResult();
        Server = host.GetTestServer();
        FixtureInstance = this;
    }

    public void Dispose() => Server.Dispose();

    private static IHostBuilder ConfigureHost()
    {
        return new HostBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder
                   .SetBasePath(context.HostingEnvironment.ContentRootPath)
                   .AddJsonFile("appsettings.json");
            })
            .UseEnvironment("Test")
            .ConfigureServices(services =>
            {
                services.ConfigureAppointmentsApiServices();
            })
            .ConfigureWebHost(webHost =>
            {
                webHost
                    .UseTestServer()
                    .Configure(application =>
                    {
                        application.UseAppointmentsApi();
                    })
                    .ConfigureTestServices(services =>
                    {
                        services.ConfigureMocks();
                    });
            });
    }
}