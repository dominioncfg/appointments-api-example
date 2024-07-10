using AppointmentsApi.Domain.Services;
using AppointmentsApi.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace AppointmentsApi.Application.Features.Appointments.Queries;

public record GetWeeklyAvaiabilityQuery : IRequest<GetWeeklyAvaiabilityQueryResponse>
{
    public DateTime Date { get; init; }
}

public record GetWeeklyAvaiabilityQueryResponse
{
    public GetWeeklyAvaiabilityFacilityQueryResponse Facility { get; init; } = new GetWeeklyAvaiabilityFacilityQueryResponse();
    public int SlotDurationMinutes { get; init; }
    public GetWeeklyAvaiabilityDaysQueryResponse Days { get; init; } = new GetWeeklyAvaiabilityDaysQueryResponse();
}

public record GetWeeklyAvaiabilityFacilityQueryResponse
{
    public Guid FacilityId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}

public record GetWeeklyAvaiabilityDaysQueryResponse
{
    public GetWeeklyAvaiabilityDayScheduleQueryResponse? Monday { get; init; }
    public GetWeeklyAvaiabilityDayScheduleQueryResponse? Tuesday { get; init; }
    public GetWeeklyAvaiabilityDayScheduleQueryResponse? Wednesday { get; init; }
    public GetWeeklyAvaiabilityDayScheduleQueryResponse? Thursday { get; init; }
    public GetWeeklyAvaiabilityDayScheduleQueryResponse? Friday { get; init; }
    public GetWeeklyAvaiabilityDayScheduleQueryResponse? Saturday { get; init; }
    public GetWeeklyAvaiabilityDayScheduleQueryResponse? Sunday { get; init; }
}


public record GetWeeklyAvaiabilityDayScheduleQueryResponse
{
    public List<GetWeeklyAvaiabilitySlotQueryResponse> FreeSlots { get; init; } = [];
}

public record GetWeeklyAvaiabilitySlotQueryResponse
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
}

public class GetWeeklyAvaiabilityQueryValidator : AbstractValidator<GetWeeklyAvaiabilityQuery>
{
    public GetWeeklyAvaiabilityQueryValidator()
    {
        RuleFor(model => model.Date)
            .NotEmpty();
    }
}

public class GetWeeklyAvaiabilityQueryHandler : IRequestHandler<GetWeeklyAvaiabilityQuery, GetWeeklyAvaiabilityQueryResponse>
{
    private readonly IAppointmentsApiClient _appointmentsApiClient;

    public GetWeeklyAvaiabilityQueryHandler(IAppointmentsApiClient appointmentsApiClient)
    {
        _appointmentsApiClient = appointmentsApiClient;
    }

    public async Task<GetWeeklyAvaiabilityQueryResponse> Handle(GetWeeklyAvaiabilityQuery request, CancellationToken cancellationToken)
    {
        var date = request.Date.Date;

        var response = await _appointmentsApiClient.GetWeeklyAvaibility(date, cancellationToken);

        var monday = GetAvailableSpots(response.Monday, date.Date, response.SlotDurationMinutes);
        var tuesday = GetAvailableSpots(response.Tuesday, date.Date.AddDays(1), response.SlotDurationMinutes);
        var wednesday = GetAvailableSpots(response.Wednesday, date.Date.AddDays(2), response.SlotDurationMinutes);
        var thursday = GetAvailableSpots(response.Thursday, date.Date.AddDays(3), response.SlotDurationMinutes);
        var friday = GetAvailableSpots(response.Friday, date.Date.AddDays(4), response.SlotDurationMinutes);
        var saturday = GetAvailableSpots(response.Saturday, date.Date.AddDays(5), response.SlotDurationMinutes);
        var sunday = GetAvailableSpots(response.Saturday, date.Date.AddDays(6), response.SlotDurationMinutes);


        return new GetWeeklyAvaiabilityQueryResponse
        {
            Facility = new GetWeeklyAvaiabilityFacilityQueryResponse()
            {
                FacilityId = response.Facility.FacilityId,
                Name = response.Facility.Name,
                Address = response.Facility.Address,
            },
            SlotDurationMinutes = response.SlotDurationMinutes,
            Days = new GetWeeklyAvaiabilityDaysQueryResponse()
            {
                Monday = monday,
                Tuesday = tuesday,
                Wednesday = wednesday,
                Thursday = thursday,
                Friday = friday,
                Saturday = saturday,
                Sunday = sunday,
            }
        };
    }

    private static GetWeeklyAvaiabilityDayScheduleQueryResponse? GetAvailableSpots(DayScheduleResponse? day, DateTime schedulingDate, int slotDurationMinutes)
    {
        if (day is null)
            return null;

        var activeWorkingPeriods = GetActiveWorkingPeriods(day, schedulingDate);
        var busySlots = day.BusySlots.Select(x => new TimePeriod(x.Start, x.End)).ToList();

        var appointmentsSlots = new List<TimePeriod>();
        foreach (var workingPeriod in activeWorkingPeriods)
        {
            var periodStart = workingPeriod.Start;
            while (periodStart < workingPeriod.End)
            {
                var slotEnd = periodStart.AddMinutes(slotDurationMinutes);
                var appointmentSlot = new TimePeriod(periodStart, slotEnd);
                if (appointmentSlot.IsContainedWithin(workingPeriod) && !busySlots.Any(x => x.OverlapsWith(appointmentSlot)))
                {
                    appointmentsSlots.Add(appointmentSlot);
                }
                periodStart = slotEnd;
            }
        }

        return new GetWeeklyAvaiabilityDayScheduleQueryResponse()
        {
            FreeSlots= appointmentsSlots.Select(x => new GetWeeklyAvaiabilitySlotQueryResponse()
            {
                Start = x.Start,
                End = x.End,
            }).ToList()
        };

    }

    private static List<TimePeriod> GetActiveWorkingPeriods(DayScheduleResponse day, DateTime schedulingDate)
    {
        var dayStartingHour = schedulingDate.AddHours(day.WorkPeriod.StartHour);
        var dayFinishingHour = schedulingDate.AddHours(day.WorkPeriod.EndHour);

        var lunchStartingHour = schedulingDate.AddHours(day.WorkPeriod.LunchStartHour);
        var lunchFinishingHour = schedulingDate.AddHours(day.WorkPeriod.LunchEndHour);

        var workingPeriod = new TimePeriod(dayStartingHour, dayFinishingHour);
        var lunchPeriod = new TimePeriod(lunchStartingHour, lunchFinishingHour);


        if (!lunchPeriod.IsContainedWithin(workingPeriod))
            return [workingPeriod];


        return
        [
            new TimePeriod(dayStartingHour,lunchStartingHour),
            new TimePeriod(lunchFinishingHour,dayFinishingHour)
        ];
    }
}