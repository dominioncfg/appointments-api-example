using AppointmentsApi.Domain.Services;

namespace AppointmentsApi.IntegrationTests.Common;

public class MockedAppointmentsApiClient : IAppointmentsApiClient
{
    private static readonly Dictionary<DateTime, AvaibilityWeeklyScheduleResponse> storage = [];
    private static readonly List<ReserveAppointmentSlotExteranalApiRequest> reservationRequestsStorage = [];

    public static void Assume(DateTime date, AvaibilityWeeklyScheduleResponse response)
    {
        storage[date] = response;
    }

    public static void ClearStorage()
    {
        storage.Clear();
        reservationRequestsStorage.Clear();
    }
    public static List<ReserveAppointmentSlotExteranalApiRequest> GetAllReservationResquest() => reservationRequestsStorage.ToList();

    public Task<AvaibilityWeeklyScheduleResponse> GetWeeklyAvaibility(DateTime monday, CancellationToken cancellationToken)
    {
        var response = storage.ContainsKey(monday.Date) ? storage[monday] : throw new ArgumentException("Not Setup for date");
        return Task.FromResult(response);
    }

    public Task ReserveAppointment(ReserveAppointmentSlotExteranalApiRequest request, CancellationToken cancellationToken)
    {
        reservationRequestsStorage.Add(request);
        return Task.CompletedTask;
    }
}
