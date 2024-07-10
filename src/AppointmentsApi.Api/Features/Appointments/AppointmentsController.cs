using AppointmentsApi.Application.Features.Appointments.Commands;
using AppointmentsApi.Application.Features.Appointments.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsApi.Api.Features.Appointments;

[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
       _mediator = mediator;
    }


    [HttpGet]
    public async Task<GetWeeklyAviabilityApiResponse> GetWeeklyAvailability(DateTime date, CancellationToken cancellationToken)
    {
        var query = new GetWeeklyAvaiabilityQuery()
        {
            Date = date,
        };
        var queryResponse = await _mediator.Send(query,cancellationToken);
        var response = GetWeeklyAviabilityApiResponse.FromQueryResponse(queryResponse);
        return response;
    }


    [HttpPost("reserve")]
    public async Task<IActionResult> TakeASlot([FromBody] ReserveAppointmentSlotApiRequest request, CancellationToken cancellationToken)
    {
        var command = new ReserveAppointmentSlotCommand()
        {
            Start = request.Start,
            Comments = request.Comments,
            End = request.End,
            FacilityId = request.FacilityId,
            Patient = new ReserveAppointmentSlotPatientCommand()
            {
                Email = request.Patient.Email,
                Name = request.Patient.Name,
                Phone = request.Patient.Phone,
                SecondName = request.Patient.SecondName,
            }
        };
        await _mediator.Send(command, cancellationToken);
        return Created("api/appointments", new() { });
    }
}

