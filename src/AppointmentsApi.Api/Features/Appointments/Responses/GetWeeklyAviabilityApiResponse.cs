using AppointmentsApi.Application.Features.Appointments.Queries;

namespace AppointmentsApi.Api.Features.Appointments;

public record GetWeeklyAviabilityApiResponse
{
   public DateTime Date { get; init; }

    public static GetWeeklyAviabilityApiResponse FromQueryResponse(GetWeeklyAvaiabilityQueryResponse response)
    {
        return new GetWeeklyAviabilityApiResponse()
        {
            Date = response.Date,
        };
    }
}
