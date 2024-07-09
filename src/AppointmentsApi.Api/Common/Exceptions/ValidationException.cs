using AppointmentsApi.Application;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentsApi.Api;

public class ValidationException : AppointmentsServiceApplicationException
{
    public IDictionary<string, string[]> Errors { get; }
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string propertyName, string error)
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>() { [propertyName] = new[] { error } };
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}