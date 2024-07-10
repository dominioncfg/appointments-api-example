using AppointmentsApi.Application.Features.Appointments.Queries;

namespace AppointmentsApi.Api.Features.Appointments;

public record GetWeeklyAviabilityApiResponse
{
    public GetWeeklyAviabilityFacilityApiResponseFacility Facility { get; init; } = new GetWeeklyAviabilityFacilityApiResponseFacility();
    public int SlotDurationMinutes { get; init; }
    public GetWeeklyAviabilityDaysApiResponse Days { get; init; } = new GetWeeklyAviabilityDaysApiResponse();

    public static GetWeeklyAviabilityApiResponse FromQueryResponse(GetWeeklyAvailabilityQueryResponse response)
    {
        return new GetWeeklyAviabilityApiResponse()
        {
            Facility = new GetWeeklyAviabilityFacilityApiResponseFacility()
            {
                FacilityId = response.Facility.FacilityId,
                Name = response.Facility.Name,
                Address = response.Facility.Address,
            },
            SlotDurationMinutes = response.SlotDurationMinutes,
            Days = new GetWeeklyAviabilityDaysApiResponse()
            {
                Monday = ConvertDayFromQueryResponse(response.Days.Monday),
                Tuesday = ConvertDayFromQueryResponse(response.Days.Tuesday),
                Wednesday = ConvertDayFromQueryResponse(response.Days.Wednesday),
                Thursday = ConvertDayFromQueryResponse(response.Days.Thursday),
                Friday = ConvertDayFromQueryResponse(response.Days.Friday),
                Saturday = ConvertDayFromQueryResponse(response.Days.Saturday),
                Sunday = ConvertDayFromQueryResponse(response.Days.Sunday),
            }
        };
    }

    private static GetWeeklyAviabilityDayScheduleApiResponse? ConvertDayFromQueryResponse(GetWeeklyAvailabilityDayScheduleQueryResponse? day)
    {
        if (day is null)
            return null;

        return new GetWeeklyAviabilityDayScheduleApiResponse()
        {
            FreeSlots = day.FreeSlots.Select(x => new GetWeeklyAviabilitySlotApiResponse()
            {
                Start = x.Start,
                End = x.End,
            }).ToList()
        };
    }
}

public record GetWeeklyAviabilityFacilityApiResponseFacility
{
    public Guid FacilityId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}

public record GetWeeklyAviabilityDaysApiResponse
{
    public GetWeeklyAviabilityDayScheduleApiResponse? Monday { get; init; }
    public GetWeeklyAviabilityDayScheduleApiResponse? Tuesday { get; init; }
    public GetWeeklyAviabilityDayScheduleApiResponse? Wednesday { get; init; }
    public GetWeeklyAviabilityDayScheduleApiResponse? Thursday { get; init; }
    public GetWeeklyAviabilityDayScheduleApiResponse? Friday { get; init; }
    public GetWeeklyAviabilityDayScheduleApiResponse? Saturday { get; init; }
    public GetWeeklyAviabilityDayScheduleApiResponse? Sunday { get; init; }
}

public record GetWeeklyAviabilityDayScheduleApiResponse
{
    public List<GetWeeklyAviabilitySlotApiResponse> FreeSlots { get; init; } = [];
}

public record GetWeeklyAviabilitySlotApiResponse
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
}
