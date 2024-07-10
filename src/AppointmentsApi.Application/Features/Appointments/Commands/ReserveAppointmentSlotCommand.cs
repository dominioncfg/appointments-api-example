using AppointmentsApi.Domain.Services;
using FluentValidation;
using MediatR;

namespace AppointmentsApi.Application.Features.Appointments.Commands;

public record ReserveAppointmentSlotCommand : IRequest
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public string Comments { get; init; } = string.Empty;
    public ReserveAppointmentSlotPatientCommand Patient { get; init; } = new();
    public Guid FacilityId { get; init; }
}

public class ReserveAppointmentSlotPatientCommand
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
        //TODO: Add More Validations Here
        RuleFor(model => model.Start)
            .NotEmpty();

        RuleFor(model => model.End)
          .NotEmpty();
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
         await _appointmentsApiClient.ReserveAppointment(sendRequest,cancellationToken);
    }
}