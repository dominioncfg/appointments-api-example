using AppointmentsApi.Domain;
using AppointmentsApi.Domain.Services;
using AppointmentsApi.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace AppointmentsApi.Application.Features.Appointments.Commands;

public record ReserveAppointmentSlotCommand : IRequest
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public string? Comments { get; init; } = string.Empty;
    public ReserveAppointmentSlotPatientCommand Patient { get; init; } = new();
    public Guid FacilityId { get; init; }
}

public record ReserveAppointmentSlotPatientCommand
{
    public string Name { get; init; } = string.Empty;
    public string SecondName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}

public class ReserveAppointmentSlotCommandValidator : AbstractValidator<ReserveAppointmentSlotCommand>
{
    public ReserveAppointmentSlotCommandValidator()
    {
        RuleFor(model => model.Start)
        .NotEmpty();

        RuleFor(model => model.End)
         .NotEmpty();

        RuleFor(model => new { model.Start, model.End })
            .Must(x => x.End > x.Start)
            .WithMessage("Must Start before Ends");

        RuleFor(model => model.Patient)
         .NotNull();

        RuleFor(model => model.Patient.Name)
        .NotNull();

        RuleFor(model => model.Patient.SecondName)
        .NotNull();

        RuleFor(model => model.Patient.Email)
        .NotNull();

        RuleFor(model => model.Patient.Phone)
        .NotNull();
    }
}

public class ReserveAppointmentSlotCommandHandler : IRequestHandler<ReserveAppointmentSlotCommand>
{
    private readonly IAppointmentsApiClient _appointmentsApiClient;

    public ReserveAppointmentSlotCommandHandler(IAppointmentsApiClient appointmentsApiClient)
    {
        _appointmentsApiClient = appointmentsApiClient;
    }

    public async Task Handle(ReserveAppointmentSlotCommand request, CancellationToken cancellationToken)
    {
        var schedulingStart = request.Start.Date.GetStartOfSchedulingWeek();
        var apiResponse = await _appointmentsApiClient.GetWeeklyAvaibility(schedulingStart, cancellationToken);
        var scheduler = WeekScheduler.FromBusySlots(schedulingStart, apiResponse);

        if (!scheduler.IsSlotFree(new TimePeriod(request.Start, request.End)))
            throw new SlotIsNotFreeApplicationException("The Slot that you requested is not available");

        var sendRequest = new ReserveAppointmentSlotExteranalApiRequest()
        {
            Start = request.Start,
            Comments = request.Comments,
            End = request.End,
            FacilityId = request.FacilityId,
            Patient = new ReserveAppointmentSlotPatientExteranalApiRequest()
            {
                Email = request.Patient.Email,
                Name = request.Patient.Name,
                Phone = request.Patient.Phone,
                SecondName = request.Patient.SecondName,
            }
        };
        await _appointmentsApiClient.ReserveAppointment(sendRequest, cancellationToken);
    }
}