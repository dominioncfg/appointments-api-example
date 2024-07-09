using FluentValidation;
using MediatR;

namespace AppointmentsApi.Application.Features.Appointments.Queries;

public record GetWeeklyAvaiabilityQuery : IRequest<GetWeeklyAvaiabilityQueryResponse>
{
    public DateTime Date { get; init; }
}

public record GetWeeklyAvaiabilityQueryResponse
{
    public DateTime Date { get; init; }
}

public class GetWeeklyAvaiabilityQueryValidator: AbstractValidator<GetWeeklyAvaiabilityQuery>
{
    public GetWeeklyAvaiabilityQueryValidator()
    {
        RuleFor(model => model.Date)
            .NotEmpty();
    }
}


public class GetWeeklyAvaiabilityQueryHandler : IRequestHandler<GetWeeklyAvaiabilityQuery, GetWeeklyAvaiabilityQueryResponse>
{

    public Task<GetWeeklyAvaiabilityQueryResponse> Handle(GetWeeklyAvaiabilityQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new GetWeeklyAvaiabilityQueryResponse
        {
            Date = request.Date
        });
    }
}