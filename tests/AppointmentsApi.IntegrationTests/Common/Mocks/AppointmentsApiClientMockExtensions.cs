using AppointmentsApi.Domain.Services;

namespace AppointmentsApi.IntegrationTests.Common;

public static class AppointmentsApiClientMockExtensions
{
    public static void AsssumeWeeklyScheduleApiClientResponseFor(this TestServerFixture _, DateTime date, AvaibilityWeeklyScheduleResponse response)
    {
        MockedAppointmentsApiClient.Assume(date, response);
    }

    public static List<ReserveAppointmentSlotExteranalApiRequest> GetSentReservationRequests(this TestServerFixture _)
    {
        return MockedAppointmentsApiClient.GetAllReservationResquest();
    }
}
