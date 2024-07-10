namespace AppointmentsApi.Domain.Services;

public record AvaibilityWeeklyScheduleResponse
{
    public FacilityResponse Facility { get; init; } = new FacilityResponse();
    public int SlotDurationMinutes { get; init; }
    public DayScheduleResponse? Monday { get; init; }
    public DayScheduleResponse? Tuesday { get; init; }
    public DayScheduleResponse? Wednesday { get; init; }
    public DayScheduleResponse? Thursday { get; init; }
    public DayScheduleResponse? Friday { get; init; }
}

public record FacilityResponse
{
    public Guid FacilityId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}

public record DayScheduleResponse
{
    public WorkPeriodResponse WorkPeriod { get; init; } = new WorkPeriodResponse();
    public List<BusySlotResponse> BusySlots { get; init; } = [];
}

public record WorkPeriodResponse
{
    public int StartHour { get; init; }
    public int EndHour { get; init; }
    public int LunchStartHour { get; init; }
    public int LunchEndHour { get; init; }
}

public record BusySlotResponse
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
}
