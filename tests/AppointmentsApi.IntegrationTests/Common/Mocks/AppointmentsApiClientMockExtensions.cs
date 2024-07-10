using AppointmentsApi.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace AppointmentsApi.IntegrationTests.Common;

public static class AppointmentsApiClientMockExtensions
{
    public static void AsssumeWeeklyScheduleReturnedByExternalApiForDate(this TestServerFixture fixture, DateTime date, AvaibilityWeeklyScheduleResponse response)
    {
        fixture.ExecuteScope(serviceProvider =>
        {
            var jsonResponse = JsonSerializer.Serialize(response);
            var handler = serviceProvider.GetRequiredService<MockHttpMessageHandler>();
            handler
                .When(HttpMethod.Get, $"{MockedAppointmentClientConfiguration.BaseUrl}/api/availability/GetWeeklyAvailability/{date:yyyyMMdd}")
                .Respond("application/json", jsonResponse);
        });
    }

    public static void AsssumeAccepetedExternalApiReservationForRequest(this TestServerFixture fixture, ReserveAppointmentSlotExteranalApiRequest expectedRequest)
    {
        fixture.ExecuteScope(serviceProvider =>
        {
            var handler = serviceProvider.GetRequiredService<MockHttpMessageHandler>();
            handler
                .When(HttpMethod.Post, $"{MockedAppointmentClientConfiguration.BaseUrl}/api/availability/TakeSlot")
                .With(request=>
                {
                    var responseJson = request.Content.ReadAsStringAsync(default).GetAwaiter().GetResult();
                    var result = JsonSerializer.Deserialize<ReserveAppointmentSlotExteranalApiRequest>(responseJson);
                    return expectedRequest == result;
                })
                .Respond(HttpStatusCode.OK);
        });
    }
}
