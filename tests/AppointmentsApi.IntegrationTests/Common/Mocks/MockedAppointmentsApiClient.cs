using AppointmentsApi.Domain.Services;

namespace AppointmentsApi.IntegrationTests.Common;

public class MockedAppointmentsApiClient : IAppointmentsApiClient
{
    private static readonly Dictionary<DateTime, AvaibilityWeeklyScheduleResponse> storage = [];


    public static void Assume(DateTime date, AvaibilityWeeklyScheduleResponse response)
    {
        storage[date] = response;
    }

    public static void ClearStorage()
    {
        storage.Clear();
    }

    public Task<AvaibilityWeeklyScheduleResponse> GetWeeklyAvaibility(DateTime monday, CancellationToken cancellationToken)
    {
        var response = storage.ContainsKey(monday.Date) ? storage[monday] : throw new ArgumentException("Not Setup for date");
        return Task.FromResult(response);
    }
}
