using AppointmentsApi.Api.Features.Appointments;
using AppointmentsApi.IntegrationTests.Common;
using FluentAssertions.Extensions;

namespace AppointmentsApi.IntegrationTests;

[Collection(nameof(TestServerFixtureCollection))]
public class WhenReservingSlots
{
    private readonly TestServerFixture Given;
    private static readonly DateTime SomeMonday = new(2024, 07, 08);

    public WhenReservingSlots(TestServerFixture given)
    {
        Given = given ?? throw new Exception("Null Server");
    }

    [Fact]
    [ResetApplicationState]
    public async Task CanReserveAppointment()
    {
        var workingSchedule = AvaibilityWeeklyScheduleResponseBuilder
             .ValidWorkingScheduleOnDayWithNoBusySlotsResponse()
             .Build();
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
        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, workingSchedule);
        AssumeAppointmentCreatedForExternalApi(validRequest);
        await PostAndExpectCreated(validRequest);
    }



    [Fact]
    [ResetApplicationState]
    public async Task DontAllowToReserveDateWhenAppointmentSlotIsNotFree()
    {
        var workingSchedule = new AvaibilityWeeklyScheduleResponseBuilder()
               .WithSlotDurationMinutes(30)
               .WithMonday(tuesday => tuesday
                   .WithWorkPeriod(period => period
                       .WithStartHour(9)
                       .WithLunchStartHour(10)
                       .WithLunchEndHour(11)
                       .WithEndHour(12)
                   )
                   .WithBusySlots(slots => slots
                       .WithBusySlot(slot => slot
                           .WithStart(SomeMonday.At(11, 30))
                           .WithEnd(SomeMonday.At(12, 00))
                       )
                   )
               )
               .Build();
        var validRequest = new ReserveAppointmentSlotApiRequestBuilder()
            .WithStart(SomeMonday.At(11, 30))
            .WithEnd(SomeMonday.At(12, 00))
            .WithComments("Testing Appointment Reservation")
            .WithFacilityId(Guid.NewGuid())
            .WithPatient(patient => patient
                .WithEmail("test@gmail.com")
                .WithName("Tester")
                .WithSecondName("Testing")
                .WithPhone("5510111")
            ).Build();


        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, workingSchedule);
        AssumeAppointmentCreatedForExternalApi(validRequest);

        await PostAndExpectBadRequest(validRequest);
    }

    public static TheoryData<DateTime> WholeWeekCases = new()
    {
        { SomeMonday },
        { SomeMonday.AddDays(1) },
        { SomeMonday.AddDays(2) },
        { SomeMonday.AddDays(3) },
        { SomeMonday.AddDays(4) },
        { SomeMonday.AddDays(5) },
        { SomeMonday.AddDays(6) },
    };

    [Theory]
    [ResetApplicationState]
    [MemberData(nameof(WholeWeekCases))]
    public async Task CanAllocateAppointmentInEveryDayOfTheWeekBasedOnTheSameWeekSchedule(DateTime date)
    {
        var workingSchedule = AvaibilityWeeklyScheduleResponseBuilder
                .ValidWorkingScheduleOnDayWithNoBusySlotsResponse()
                .Build();
        var validRequest = new ReserveAppointmentSlotApiRequestBuilder()
            .WithStart(date.At(09, 00))
            .WithEnd(date.At(09, 30))
            .WithComments("Testing Appointment Reservation")
            .WithFacilityId(Guid.NewGuid())
            .WithPatient(patient => patient
                .WithEmail("test@gmail.com")
                .WithName("Tester")
                .WithSecondName("Testing")
                .WithPhone("5510111")
            ).Build();
        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, workingSchedule);
        AssumeAppointmentCreatedForExternalApi(validRequest);
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

    private void AssumeAppointmentCreatedForExternalApi(ReserveAppointmentSlotApiRequest validRequest)
    {
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