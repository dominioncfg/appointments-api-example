namespace AppointmentsApi.Domain.Services;

public record ReserveAppointmentSlotExteranalApiRequest
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public string Comments { get; init; } = string.Empty;
    public ReserveAppointmentSlotPatientExteranalApiRequest Patient { get; init; } = new();
    public Guid FacilityId { get; init; }
}

public class ReserveAppointmentSlotPatientExteranalApiRequest
{
    public string Name { get; init; } = string.Empty;
    public string SecondName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}

