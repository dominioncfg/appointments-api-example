using AppointmentsApi.Api.Features.Appointments;
using AppointmentsApi.IntegrationTests.Seedwork;

namespace AppointmentsApi.IntegrationTests;

[Collection(nameof(TestServerFixtureCollection))]
public class WhenGettingWeeklyAviability
{
    private readonly TestServerFixture Given;

    public WhenGettingWeeklyAviability(TestServerFixture given)
    {
        Given = given ?? throw new Exception("Null Server");
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsOkayForNow()
    {
        var date = DateTime.Today;
        var url = GetAppointmentsUrl(date);
        var response = await Given.Server.CreateClient().GetAsync<GetWeeklyAviabilityApiResponse>(url);

        response.Should().NotBeNull();
        response.Date.Should().Be(date);
    }


    [Fact]
    [ResetApplicationState]
    public async Task ReturnsBadRequestForDefaultDate()
    {
        var date = DateTime.MinValue;
        var url = GetAppointmentsUrl(date);
        await Given.Server.CreateClient().GetAndExpectBadRequestAsync(url);
    }

    private static string GetAppointmentsUrl(DateTime date) => $"api/appointments?date={date.ToString("yyyy-MM-dd")}";
}
