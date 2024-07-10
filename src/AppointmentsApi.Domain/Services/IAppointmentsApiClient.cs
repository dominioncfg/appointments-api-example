namespace AppointmentsApi.Domain.Services;

public interface IAppointmentsApiClient
{
    public Task<AvaibilityWeeklyScheduleResponse> GetWeeklyAvaibility(DateTime monday, CancellationToken cancellationToken);

    public Task ReserveAppointment(ReserveAppointmentSlotExteranalApiRequest request, CancellationToken cancellationToken);
}

