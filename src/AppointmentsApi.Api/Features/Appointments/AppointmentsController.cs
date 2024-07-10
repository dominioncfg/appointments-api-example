using AppointmentsApi.Application.Features.Appointments.Commands;
using AppointmentsApi.Application.Features.Appointments.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
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



    /// <summary>
    /// Gets the weekly schedule of appointmentss of the current Facility.
    /// </summary>
    /// /// <param name="date">Can be any day and we will show you the schedule for that week</param>
    /// <returns>List of days with the available slots.</returns>
    /// <response code="200">List of days with the available slots.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<GetWeeklyAviabilityApiResponse> GetWeeklyAvailability(DateTime date, CancellationToken cancellationToken)
    {
        var query = new GetWeeklyAvailabilityQuery()
        {
            Date = date,
        };
        var queryResponse = await _mediator.Send(query,cancellationToken);
        var response = GetWeeklyAviabilityApiResponse.FromQueryResponse(queryResponse);
        return response;
    }



    /// <summary>
    /// Allows the user to request a appoinment in the current facility.
    /// </summary>
    /// <returns>No result if everything goes well</returns>
    /// <response code="201">Appointment Allocated</response>
    /// <response code="400">If the spot is already taken.</response>
    [HttpPost("reserve")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

