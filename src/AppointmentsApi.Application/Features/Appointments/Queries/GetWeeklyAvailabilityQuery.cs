using AppointmentsApi.Domain;
using AppointmentsApi.Domain.Services;
using FluentValidation;
using MediatR;

namespace AppointmentsApi.Application.Features.Appointments.Queries;

public record GetWeeklyAvailabilityQuery : IRequest<GetWeeklyAvailabilityQueryResponse>
{
    public DateTime Date { get; init; }
}

public record GetWeeklyAvailabilityQueryResponse
{
    public GetWeeklyAvailabilityFacilityQueryResponse Facility { get; init; } = new GetWeeklyAvailabilityFacilityQueryResponse();
    public int SlotDurationMinutes { get; init; }
    public GetWeeklyAvailabilityDaysQueryResponse Days { get; init; } = new GetWeeklyAvailabilityDaysQueryResponse();
}

public record GetWeeklyAvailabilityFacilityQueryResponse
{
    public Guid FacilityId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}

public record GetWeeklyAvailabilityDaysQueryResponse
{
    public GetWeeklyAvailabilityDayScheduleQueryResponse? Monday { get; init; }
    public GetWeeklyAvailabilityDayScheduleQueryResponse? Tuesday { get; init; }
    public GetWeeklyAvailabilityDayScheduleQueryResponse? Wednesday { get; init; }
    public GetWeeklyAvailabilityDayScheduleQueryResponse? Thursday { get; init; }
    public GetWeeklyAvailabilityDayScheduleQueryResponse? Friday { get; init; }
    public GetWeeklyAvailabilityDayScheduleQueryResponse? Saturday { get; init; }
    public GetWeeklyAvailabilityDayScheduleQueryResponse? Sunday { get; init; }
}


public record GetWeeklyAvailabilityDayScheduleQueryResponse
{
    public List<GetWeeklyAvailabilitySlotQueryResponse> FreeSlots { get; init; } = [];
}

public record GetWeeklyAvailabilitySlotQueryResponse
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
}

public class GetWeeklyAvailabilityQueryValidator : AbstractValidator<GetWeeklyAvailabilityQuery>
{
    public GetWeeklyAvailabilityQueryValidator()
    {
        RuleFor(model => model.Date)
            .NotEmpty();
    }
}

public class GetWeeklyAvailabilityQueryHandler : IRequestHandler<GetWeeklyAvailabilityQuery, GetWeeklyAvailabilityQueryResponse>
{
    private readonly IAppointmentsApiClient _appointmentsApiClient;

    public GetWeeklyAvailabilityQueryHandler(IAppointmentsApiClient appointmentsApiClient)
    {
        _appointmentsApiClient = appointmentsApiClient;
    }

    public async Task<GetWeeklyAvailabilityQueryResponse> Handle(GetWeeklyAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var date =  request.Date.GetStartOfSchedulingWeek();

        var response = await _appointmentsApiClient.GetWeeklyAvaibility(date, cancellationToken);

        var scheduler = WeekScheduler.FromBusySlots(date, response);

        return new GetWeeklyAvailabilityQueryResponse
        {
            Facility = new GetWeeklyAvailabilityFacilityQueryResponse()
            {
                FacilityId = response.Facility.FacilityId,
                Name = response.Facility.Name,
                Address = response.Facility.Address,
            },
            SlotDurationMinutes = response.SlotDurationMinutes,
            Days = new GetWeeklyAvailabilityDaysQueryResponse()
            {
                Monday = ConvertDayFromDomain(scheduler.Monday),
                Tuesday = ConvertDayFromDomain(scheduler.Tuesday),
                Wednesday = ConvertDayFromDomain(scheduler.Wednesday),
                Thursday = ConvertDayFromDomain(scheduler.Thursday),
                Friday = ConvertDayFromDomain(scheduler.Friday),
                Saturday = ConvertDayFromDomain(scheduler.Saturday),
                Sunday = ConvertDayFromDomain(scheduler.Sunday),
            }
        };
    }

    private static GetWeeklyAvailabilityDayScheduleQueryResponse? ConvertDayFromDomain(DaySchedule? daySchedule)
    {
        if(daySchedule is null)
            return null;

        return new GetWeeklyAvailabilityDayScheduleQueryResponse()
        {
            FreeSlots = daySchedule.FreeSlots.Select(x => new GetWeeklyAvailabilitySlotQueryResponse()
            {
                Start = x.Start,
                End = x.End,
            }).ToList(),
        };
    }
}