namespace AppointmentsApi.Infrastructure.Services;

public record AppointmentsApiConfiguration
{
    public const string SectionName = "AppointmentsApi";
    public string BaseUrl { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
