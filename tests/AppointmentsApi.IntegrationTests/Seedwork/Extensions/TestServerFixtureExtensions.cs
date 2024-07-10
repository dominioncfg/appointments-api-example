using Microsoft.Extensions.DependencyInjection;

namespace AppointmentsApi.IntegrationTests.Seedwork;

public static class TestServerFixtureExtensions
{
    public static async Task ExecuteScopeAsync(this TestServerFixture fixture, Func<IServiceProvider, Task> function)
    {
        var scopeFactory = fixture.Server.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        await function(scope.ServiceProvider);
    }

    public static void ExecuteScope(this TestServerFixture fixture, Action<IServiceProvider> action)
    {
        var scopeFactory = fixture.Server.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        action(scope.ServiceProvider);
    }
}