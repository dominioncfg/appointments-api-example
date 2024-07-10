namespace AppointmentsApi.Api.Features.Appointments;

public record ReserveAppointmentSlotApiRequest
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public string? Comments { get; init; } = string.Empty;
    public ReserveppointmentSlottPatientApiRequest Patient { get; init; } = new();
    public Guid FacilityId { get; init; }
}

public class ReserveppointmentSlottPatientApiRequest 
{
    public string Name { get; init; } = string.Empty;
    public string SecondName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}
