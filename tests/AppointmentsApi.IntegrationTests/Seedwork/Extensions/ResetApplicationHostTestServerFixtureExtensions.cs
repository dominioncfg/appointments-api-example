namespace AppointmentsApi.IntegrationTests.Seedwork;

public static class ResetApplicationHostTestServerFixtureExtensions
{
    public static void OnTestInitResetApplicationServices(this TestServerFixture server)
    {
        server.ResetMocks();
    }
}
