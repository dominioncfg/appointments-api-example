using AppointmentsApi.Domain.Services;
using System.Net.Mime;
using System.Net;
using System.Text;
using System;
using System.Text.Json;

namespace AppointmentsApi.Infrastructure.Services;


public class AppointmentsApiClient : IAppointmentsApiClient
{
    private readonly HttpClient _client;

    public AppointmentsApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<AvaibilityWeeklyScheduleResponse> GetWeeklyAvaibility(DateTime monday, CancellationToken cancellationToken)
    {
        var getUrl = $"api/availability/GetWeeklyAvailability/{monday:yyyyMMdd}";
        var response = await _client.GetAsync(getUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

        var result = JsonSerializer.Deserialize<AvaibilityWeeklyScheduleResponse>(responseJson);

        return result is null ? throw new System.Exception("The Response could not be parsed.") : result;

    }

    public async Task ReserveAppointment(ReserveAppointmentSlotExteranalApiRequest request, CancellationToken cancellationToken)
    {
        var postUrl = $"api/availability/TakeSlot";
        var json = JsonSerializer.Serialize(request);
        var httpContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _client.PostAsync(postUrl, httpContent);
        response.EnsureSuccessStatusCode();
    }
}
