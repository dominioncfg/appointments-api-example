using AppointmentsApi.Api.Features.Appointments;
using AppointmentsApi.IntegrationTests.Common;
using FluentAssertions.Extensions;

namespace AppointmentsApi.IntegrationTests;

[Collection(nameof(TestServerFixtureCollection))]
public class WhenGettingWeeklyAviability
{
    private readonly TestServerFixture Given;
    private readonly DateTime SomeMonday = new(2024, 07, 08);
    private DateTime SomeTuesday => SomeMonday.AddDays(1);
    private DateTime SomeWednesday => SomeMonday.AddDays(2);
    private DateTime SomeThursday => SomeMonday.AddDays(3);

    public WhenGettingWeeklyAviability(TestServerFixture given)
    {
        Given = given ?? throw new Exception("Null Server");
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsFacilityInformation()
    {
        var apiResponse = AvaibilityWeeklyScheduleResponseBuilder.ValidWorkingScheduleResponse()
             .WithFacility(facility => facility
                .WithFacilityId(Guid.NewGuid())
                .WithName("Las Palmeras")
                .WithAddress("Plaza de la independencia 36, 38006 Santa Cruz de Tenerife")
            ).Build();
        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, apiResponse);

        var url = GetAppointmentsUrl(SomeMonday);
        var response = await Given.Server.CreateClient().GetAsync<GetWeeklyAviabilityApiResponse>(url);

        response.Should().NotBeNull();

        response.Facility.Should().NotBeNull();

        response.Facility.FacilityId.Should().Be(apiResponse.Facility.FacilityId);
        response.Facility.Name.Should().Be(apiResponse.Facility.Name);
        response.Facility.Address.Should().Be(apiResponse.Facility.Address);
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsSlotDurationMinutes()
    {
        var apiResponse = AvaibilityWeeklyScheduleResponseBuilder.ValidWorkingScheduleResponse()
             .WithSlotDurationMinutes(30)
             .Build();
        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, apiResponse);

        var url = GetAppointmentsUrl(SomeMonday);
        var response = await Given.Server.CreateClient().GetAsync<GetWeeklyAviabilityApiResponse>(url);

        response.Should().NotBeNull();

        response.SlotDurationMinutes.Should().Be(apiResponse.SlotDurationMinutes);
    }


    [Fact]
    [ResetApplicationState]
    public async Task ReturnsFreeSpotsForSingleDayNoBusySlots()
    {
        var apiResponse = new AvaibilityWeeklyScheduleResponseBuilder()
            .WithSlotDurationMinutes(30)
            .WithMonday(monday => monday
                .WithWorkPeriod(period => period
                    .WithStartHour(9)
                    .WithLunchStartHour(10)
                    .WithLunchEndHour(11)
                    .WithEndHour(12)
                )
            )
            .Build();
        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, apiResponse);

        var url = GetAppointmentsUrl(SomeMonday);
        var response = await Given.Server.CreateClient().GetAsync<GetWeeklyAviabilityApiResponse>(url);

        response.Days.Should().NotBeNull();
        response.Days.Tuesday.Should().BeNull();
        response.Days.Wednesday.Should().BeNull();
        response.Days.Thursday.Should().BeNull();
        response.Days.Friday.Should().BeNull();
        response.Days.Saturday.Should().BeNull();
        response.Days.Sunday.Should().BeNull();

        response.Days.Monday.Should().NotBeNull();
        response.Days.Monday!.FreeSlots.Should().NotBeNullOrEmpty().And.HaveCount(4);

        var firstSlot = response.Days.Monday.FreeSlots[0];
        firstSlot.Should().NotBeNull();
        firstSlot.Start.Should().Be(SomeMonday.At(9, 00));
        firstSlot.End.Should().Be(SomeMonday.At(9, 30));

        var secondSlot = response.Days.Monday.FreeSlots[1];
        secondSlot.Should().NotBeNull();
        secondSlot.Start.Should().Be(SomeMonday.At(9, 30));
        secondSlot.End.Should().Be(SomeMonday.At(10, 00));

        var thirdSlot = response.Days.Monday.FreeSlots[2];
        thirdSlot.Should().NotBeNull();
        thirdSlot.Start.Should().Be(SomeMonday.At(11, 00));
        thirdSlot.End.Should().Be(SomeMonday.At(11, 30));

        var fourthSlot = response.Days.Monday.FreeSlots[3];
        fourthSlot.Should().NotBeNull();
        fourthSlot.Start.Should().Be(SomeMonday.At(11, 30));
        fourthSlot.End.Should().Be(SomeMonday.At(12, 00));
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsFreeSpotsForSingleDayOneBusySlotAtEndOfDay()
    {
        var apiResponse = new AvaibilityWeeklyScheduleResponseBuilder()
                .WithSlotDurationMinutes(30)
                .WithTuesday(tuesday => tuesday
                    .WithWorkPeriod(period => period
                        .WithStartHour(9)
                        .WithLunchStartHour(10)
                        .WithLunchEndHour(11)
                        .WithEndHour(12)
                    )
                    .WithBusySlots(slots => slots
                        .WithBusySlot(slot => slot
                            .WithStart(SomeTuesday.At(11, 30))
                            .WithEnd(SomeTuesday.At(12, 00))
                        )
                    )
                )
                .Build();
        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, apiResponse);

        var url = GetAppointmentsUrl(SomeMonday);
        var response = await Given.Server.CreateClient().GetAsync<GetWeeklyAviabilityApiResponse>(url);

        response.Days.Should().NotBeNull();
        response.Days.Monday.Should().BeNull();
        response.Days.Wednesday.Should().BeNull();
        response.Days.Thursday.Should().BeNull();
        response.Days.Friday.Should().BeNull();
        response.Days.Saturday.Should().BeNull();
        response.Days.Sunday.Should().BeNull();

        response.Days.Tuesday.Should().NotBeNull();
        response.Days.Tuesday!.FreeSlots.Should().NotBeNullOrEmpty().And.HaveCount(3);

        var firstSlot = response.Days.Tuesday.FreeSlots[0];
        firstSlot.Should().NotBeNull();
        firstSlot.Start.Should().Be(SomeTuesday.At(9, 00));
        firstSlot.End.Should().Be(SomeTuesday.At(9, 30));

        var secondSlot = response.Days.Tuesday.FreeSlots[1];
        secondSlot.Should().NotBeNull();
        secondSlot.Start.Should().Be(SomeTuesday.At(9, 30));
        secondSlot.End.Should().Be(SomeTuesday.At(10, 00));

        var thirdSlot = response.Days.Tuesday.FreeSlots[2];
        thirdSlot.Should().NotBeNull();
        thirdSlot.Start.Should().Be(SomeTuesday.At(11, 00));
        thirdSlot.End.Should().Be(SomeTuesday.At(11, 30));
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsFreeSpotsForSingleDayOneBusySlotAtBeginingOfDay()
    {
        var apiResponse = new AvaibilityWeeklyScheduleResponseBuilder()
                .WithSlotDurationMinutes(30)
                .WithTuesday(tuesday => tuesday
                    .WithWorkPeriod(period => period
                        .WithStartHour(9)
                        .WithLunchStartHour(10)
                        .WithLunchEndHour(11)
                        .WithEndHour(12)
                    )
                    .WithBusySlots(slots => slots
                        .WithBusySlot(slot => slot
                            .WithStart(SomeTuesday.At(9, 00))
                            .WithEnd(SomeTuesday.At(9, 30))
                        )
                    )
                )
                .Build();
        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, apiResponse);

        var url = GetAppointmentsUrl(SomeMonday);
        var response = await Given.Server.CreateClient().GetAsync<GetWeeklyAviabilityApiResponse>(url);

        response.Days.Should().NotBeNull();
        response.Days.Monday.Should().BeNull();
        response.Days.Wednesday.Should().BeNull();
        response.Days.Thursday.Should().BeNull();
        response.Days.Friday.Should().BeNull();
        response.Days.Saturday.Should().BeNull();
        response.Days.Sunday.Should().BeNull();

        response.Days.Tuesday.Should().NotBeNull();
        response.Days.Tuesday!.FreeSlots.Should().NotBeNullOrEmpty().And.HaveCount(3);

        var firstSlot = response.Days.Tuesday.FreeSlots[0];
        firstSlot.Should().NotBeNull();
        firstSlot.Start.Should().Be(SomeTuesday.At(9, 30));
        firstSlot.End.Should().Be(SomeTuesday.At(10, 00));

        var secondSlot = response.Days.Tuesday.FreeSlots[1];
        secondSlot.Should().NotBeNull();
        secondSlot.Start.Should().Be(SomeTuesday.At(11, 00));
        secondSlot.End.Should().Be(SomeTuesday.At(11, 30));

        var thirdSlot = response.Days.Tuesday.FreeSlots[2];
        thirdSlot.Should().NotBeNull();
        thirdSlot.Start.Should().Be(SomeTuesday.At(11, 30));
        thirdSlot.End.Should().Be(SomeTuesday.At(12, 00));
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsFreeSlotsWhenLunchPeriodIsAtTheEndOfDay()
    {
        var apiResponse = new AvaibilityWeeklyScheduleResponseBuilder()
            .WithSlotDurationMinutes(30)
            .WithWednesday(wednesday => wednesday
                .WithWorkPeriod(period => period
                    .WithStartHour(9)
                    .WithLunchStartHour(10)
                    .WithLunchEndHour(11)
                    .WithEndHour(11)
                )
            )
            .Build();
        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, apiResponse);

        var url = GetAppointmentsUrl(SomeMonday);
        var response = await Given.Server.CreateClient().GetAsync<GetWeeklyAviabilityApiResponse>(url);

        response.Should().NotBeNull();

        response.Days.Should().NotBeNull();
        response.Days.Monday.Should().BeNull();
        response.Days.Tuesday.Should().BeNull();
        response.Days.Thursday.Should().BeNull();
        response.Days.Friday.Should().BeNull();
        response.Days.Saturday.Should().BeNull();
        response.Days.Sunday.Should().BeNull();

        response.Days.Wednesday.Should().NotBeNull();
        response.Days.Wednesday!.FreeSlots.Should().NotBeNullOrEmpty().And.HaveCount(2);

        var firstSlot = response.Days.Wednesday.FreeSlots[0];
        firstSlot.Should().NotBeNull();
        firstSlot.Start.Should().Be(SomeWednesday.At(9, 00));
        firstSlot.End.Should().Be(SomeWednesday.At(9, 30));

        var secondSlot = response.Days.Wednesday.FreeSlots[1];
        secondSlot.Should().NotBeNull();
        secondSlot.Start.Should().Be(SomeWednesday.At(9, 30));
        secondSlot.End.Should().Be(SomeWednesday.At(10, 00));
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsFreeSlotsWhenLunchPeriodIsAtTheBeginningOfDay()
    {
        var apiResponse = new AvaibilityWeeklyScheduleResponseBuilder()
            .WithSlotDurationMinutes(30)
            .WithThursday(thursday => thursday
                .WithWorkPeriod(period => period
                    .WithStartHour(9)
                    .WithLunchStartHour(9)
                    .WithLunchEndHour(10)
                    .WithEndHour(11)
                )
            )
            .Build();
        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, apiResponse);

        var url = GetAppointmentsUrl(SomeMonday);
        var response = await Given.Server.CreateClient().GetAsync<GetWeeklyAviabilityApiResponse>(url);

        response.Should().NotBeNull();

        response.Days.Should().NotBeNull();
        response.Days.Monday.Should().BeNull();
        response.Days.Tuesday.Should().BeNull();
        response.Days.Wednesday.Should().BeNull();
        response.Days.Friday.Should().BeNull();
        response.Days.Saturday.Should().BeNull();
        response.Days.Sunday.Should().BeNull();

        response.Days.Thursday.Should().NotBeNull();
        response.Days.Thursday!.FreeSlots.Should().NotBeNullOrEmpty().And.HaveCount(2);

        var firstSlot = response.Days.Thursday.FreeSlots[0];
        firstSlot.Should().NotBeNull();
        firstSlot.Start.Should().Be(SomeThursday.At(10, 00));
        firstSlot.End.Should().Be(SomeThursday.At(10, 30));

        var secondSlot = response.Days.Thursday.FreeSlots[1];
        secondSlot.Should().NotBeNull();
        secondSlot.Start.Should().Be(SomeThursday.At(10, 30));
        secondSlot.End.Should().Be(SomeThursday.At(11, 00));
    }

    [Fact]
    [ResetApplicationState]
    public async Task DoesNotRetunsSlotsWhenEndsAfterLunchStart()
    {
        var apiResponse = new AvaibilityWeeklyScheduleResponseBuilder()
            .WithSlotDurationMinutes(65)
            .WithFriday(thursday => thursday
                .WithWorkPeriod(period => period
                    .WithStartHour(9)
                    .WithLunchStartHour(10)
                    .WithLunchEndHour(11)
                    .WithEndHour(11)
                )
            )
            .Build();
        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, apiResponse);

        var url = GetAppointmentsUrl(SomeMonday);
        var response = await Given.Server.CreateClient().GetAsync<GetWeeklyAviabilityApiResponse>(url);

        response.Should().NotBeNull();

        response.Days.Should().NotBeNull();
        response.Days.Monday.Should().BeNull();
        response.Days.Tuesday.Should().BeNull();
        response.Days.Wednesday.Should().BeNull();
        response.Days.Thursday.Should().BeNull();
        response.Days.Saturday.Should().BeNull();
        response.Days.Sunday.Should().BeNull();

        response.Days.Friday.Should().NotBeNull();
        response.Days.Friday!.FreeSlots.Should().NotBeNull().And.BeEmpty();
    }


    [Theory]
    [ResetApplicationState]
    [InlineData(59,1)]
    [InlineData(60, 1)]
    [InlineData(61, 0)]
    public async Task DoesNotRetunsSlotsWhenEndsAfterEndOfDay(int expirationTime, int expectedCount)
    {
        var apiResponse = new AvaibilityWeeklyScheduleResponseBuilder()
            .WithSlotDurationMinutes(expirationTime)
            .WithThursday(thursday => thursday
                .WithWorkPeriod(period => period
                    .WithStartHour(9)
                    .WithLunchStartHour(09)
                    .WithLunchEndHour(10)
                    .WithEndHour(11)
                )
            )
            .Build();
        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, apiResponse);

        var url = GetAppointmentsUrl(SomeMonday);
        var response = await Given.Server.CreateClient().GetAsync<GetWeeklyAviabilityApiResponse>(url);

        response.Should().NotBeNull();

        response.Days.Should().NotBeNull();
        response.Days.Monday.Should().BeNull();
        response.Days.Tuesday.Should().BeNull();
        response.Days.Wednesday.Should().BeNull();
        response.Days.Friday.Should().BeNull();
        response.Days.Saturday.Should().BeNull();
        response.Days.Sunday.Should().BeNull();

        response.Days.Thursday.Should().NotBeNull();
        response.Days.Thursday!.FreeSlots.Should().NotBeNull().And.HaveCount(expectedCount);
    }


    [Fact]
    [ResetApplicationState]
    public async Task ReturnsBadRequestForDefaultDate()
    {
        var date = DateTime.MinValue;
        var url = GetAppointmentsUrl(date);
        await Given.Server.CreateClient().GetAndExpectBadRequestAsync(url);
    }

    private static string GetAppointmentsUrl(DateTime date) => $"api/appointments?date={date:yyyy-MM-dd}";
}



