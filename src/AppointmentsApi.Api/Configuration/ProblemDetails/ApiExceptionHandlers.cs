using AppointmentsApi.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsApi.Api;

internal static class ApiExceptionHandlers
{
    private static string UnhandledExceptionTitle => "Whoops. Something went wrong";
    private static string ValidationExceptionTitle => "One or more validation failures have occurred.";
    private static string BadRequestExceptionTitle => "Looks like there is something wrong with your request.";

    public static ProblemDetails UnhandledExceptionHandler(Exception ex)
    {
        return new ProblemDetails
        {
            Detail = ex.Message,
            Status = StatusCodes.Status500InternalServerError,
            Title = UnhandledExceptionTitle
        };
    }

    public static ProblemDetails FluentValidationExceptionHandler(ValidationException ex)
    {
        return new ValidationProblemDetails(ex.Errors)
        {
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest,
            Title = ValidationExceptionTitle,
        };
    }

    public static ProblemDetails ApplicationExceptionHandler(AppointmentsServiceApplicationException ex)
    {
        return new ValidationProblemDetails()
        {
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest,
            Title = BadRequestExceptionTitle,
        };
    }
}