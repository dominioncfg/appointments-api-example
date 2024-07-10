using AppointmentsApi.Api.Features.Appointments;
using AppointmentsApi.IntegrationTests.Common;
using FluentAssertions.Extensions;

namespace AppointmentsApi.IntegrationTests;

[Collection(nameof(TestServerFixtureCollection))]
public class WhenReservingSlots
{
    private readonly TestServerFixture Given;
    private readonly DateTime SomeMonday = new(2024, 07, 08);

    public WhenReservingSlots(TestServerFixture given)
    {
        Given = given ?? throw new Exception("Null Server");
    }

    [Fact]
    [ResetApplicationState]
    public async Task CanReserveAppointment()
    {

        var validRequest = new ReserveAppointmentSlotApiRequestBuilder()
            .WithStart(SomeMonday.At(09, 00))
            .WithEnd(SomeMonday.At(09, 30))
            .WithComments("Testing Appointment Reservation")
            .WithFacilityId(Guid.NewGuid())
            .WithPatient(patient => patient
                .WithEmail("test@gmail.com")
                .WithName("Tester")
                .WithSecondName("Testing")
                .WithPhone("5510111")
            ).Build();

        var expectedExternalApiRequest = new ReserveAppointmentSlotExteranalApiRequestBuilder()
           .WithStart(validRequest.Start)
           .WithEnd(validRequest.End)
           .WithComments(validRequest.Comments)
           .WithFacilityId(validRequest.FacilityId)
           .WithPatient(patient => patient
               .WithEmail(validRequest.Patient.Email)
               .WithName(validRequest.Patient.Name)
               .WithSecondName(validRequest.Patient.SecondName)
               .WithPhone(validRequest.Patient.Phone)
           ).Build();
        Given.AsssumeAccepetedExternalApiReservationForRequest(expectedExternalApiRequest);
        await PostAndExpectCreated(validRequest);
    }


    [Fact]
    [ResetApplicationState]
    public async Task ReturnsBadRequestWhenDefauDate()
    {
        var inValidRequest = new ReserveAppointmentSlotApiRequestBuilder()
            .WithStart(DateTime.MinValue)
            .WithEnd(SomeMonday.At(09, 30))
            .WithComments("Testing Appointment Reservation")
            .WithFacilityId(Guid.NewGuid())
            .WithPatient(patient => patient
                .WithEmail("test@gmail.com")
                .WithName("Tester")
                .WithSecondName("Testing")
                .WithPhone("5510111")
            ).Build();
        await PostAndExpectBadRequest(inValidRequest);
    }

    private async Task PostAndExpectCreated(ReserveAppointmentSlotApiRequest request)
    {
        var url = ReserveAppointmentsUrl();
        await Given.Server.CreateClient().PostAndExpectCreatedAsync(url, request);
    }

    private async Task PostAndExpectBadRequest(ReserveAppointmentSlotApiRequest request)
    {
        var url = ReserveAppointmentsUrl();
        await Given.Server.CreateClient().PostAndExpectBadRequestAsync(url, request);
    }
    
    private static string ReserveAppointmentsUrl() => $"api/appointments/reserve";
}