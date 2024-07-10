using AppointmentsApi.Domain;
using AppointmentsApi.Domain.Services;
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
        var date =  request.Date.GetStartOfSchedulingWeek();

        var response = await _appointmentsApiClient.GetWeeklyAvaibility(date, cancellationToken);

        var scheduler = WeekScheduler.FromBusySlots(date, response);

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

    private static GetWeeklyAvaiabilityDayScheduleQueryResponse? ConvertDayFromDomain(DaySchedule? daySchedule)
    {
        if(daySchedule is null)
            return null;

        return new GetWeeklyAvaiabilityDayScheduleQueryResponse()
        {
            FreeSlots = daySchedule.FreeSlots.Select(x => new GetWeeklyAvaiabilitySlotQueryResponse()
            {
                Start = x.Start,
                End = x.End,
            }).ToList(),
        };
    }
}