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
}

