namespace AppointmentsApi.Domain.Services;

public interface IAppointmentsApiClient
{
    public Task<AvaibilityWeeklyScheduleResponse> GetWeeklyAvaibility(DateTime monday, CancellationToken cancellationToken);
}

