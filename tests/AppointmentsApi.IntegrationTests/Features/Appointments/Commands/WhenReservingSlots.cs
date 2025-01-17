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
        var validRequest = GivenValidRequest().Build();
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
        var validRequest = GivenValidRequest()
            .WithStart(SomeMonday.At(11, 30))
            .WithEnd(SomeMonday.At(12, 00))
            .Build();


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

        var validRequest = GivenValidRequest()
            .WithStart(date.At(09, 00))
            .WithEnd(date.At(09, 30))
            .Build();
       
        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, workingSchedule);
        AssumeAppointmentCreatedForExternalApi(validRequest);
        await PostAndExpectCreated(validRequest);
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsBadRequestWhenDefaultStartDate()
    {
        var inValidRequest = GivenValidRequest()
            .WithStart(DateTime.MinValue)
            .Build();
        await PostAndExpectBadRequest(inValidRequest);
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsBadRequestWhenDefaultEndDate()
    {
        var inValidRequest = GivenValidRequest()
            .WithEnd(DateTime.MinValue)
            .Build();
        await PostAndExpectBadRequest(inValidRequest);
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsBadRequestWhenEndDateIsBeforeStartDate()
    {
        var inValidRequest = GivenValidRequest()
            .WithStart(SomeMonday.At(10, 00))
            .WithEnd(SomeMonday.At(9,00))
            .Build();
        await PostAndExpectBadRequest(inValidRequest);
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsBadRequestWhenNameIsNotSent()
    {
        var inValidRequest = GivenValidRequest()
            .WithPatient(x=>x
                .WithName(null!)
                .WithPhone("551001")
                .WithEmail("juan@gmail.com")
                .WithSecondName("SN")
            )
            .Build();
        await PostAndExpectBadRequest(inValidRequest);
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsBadRequestWhenPhoneIsNotSent()
    {
        var inValidRequest = GivenValidRequest()
            .WithPatient(x => x
                .WithName("FN")
                .WithPhone(null!)
                .WithEmail("juan@gmail.com")
                .WithSecondName("SN")
            )
            .Build();
        await PostAndExpectBadRequest(inValidRequest);
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsBadRequestWhenEmailIsNotSent()
    {
        var inValidRequest = GivenValidRequest()
            .WithPatient(x => x
                .WithName("FN")
                .WithPhone("551001")
                .WithEmail(null!)
                .WithSecondName("SN")
            )
            .Build();
        await PostAndExpectBadRequest(inValidRequest);
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsBadRequestWhenSecondNameIsNotSent()
    {
        var inValidRequest = GivenValidRequest()
            .WithPatient(x => x
                .WithName("FN")
                .WithPhone("551001")
                .WithEmail("juan@gmail.com")
                .WithSecondName(null!)
            )
            .Build();
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

    private static ReserveAppointmentSlotApiRequestBuilder GivenValidRequest()
    {
        return new ReserveAppointmentSlotApiRequestBuilder()
            .WithStart(SomeMonday.At(09, 00))
            .WithEnd(SomeMonday.At(09, 30))
            .WithComments("Testing Appointment Reservation")
            .WithFacilityId(Guid.NewGuid())
            .WithPatient(patient => patient
                .WithEmail("test@gmail.com")
                .WithName("Tester")
                .WithSecondName("Testing")
                .WithPhone("5510111")
            );
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