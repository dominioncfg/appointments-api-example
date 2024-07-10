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
        await PostAndExpectCreated(validRequest);

        var reservationRequestsSent = Given.GetSentReservationRequests();
        reservationRequestsSent.Should().NotBeNull().And.HaveCount(1);
        var requestToExternalApi = reservationRequestsSent.First();

        requestToExternalApi.Should().NotBeNull();
        requestToExternalApi.Start.Should().Be(validRequest.Start);
        requestToExternalApi.End.Should().Be(validRequest.End);
        requestToExternalApi.Comments.Should().Be(validRequest.Comments);
        requestToExternalApi.FacilityId.Should().Be(validRequest.FacilityId);
        
        requestToExternalApi.Patient.Should().NotBeNull();
        requestToExternalApi.Patient.Email.Should().Be(validRequest.Patient.Email);
        requestToExternalApi.Patient.Name.Should().Be(validRequest.Patient.Name);
        requestToExternalApi.Patient.SecondName.Should().Be(validRequest.Patient.SecondName);
        requestToExternalApi.Patient.Phone.Should().Be(validRequest.Patient.Phone);
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



