using AppointmentsApi.Api.Features.Appointments;
using AppointmentsApi.IntegrationTests.Common;
using FluentAssertions.Extensions;

namespace AppointmentsApi.IntegrationTests;

[Collection(nameof(TestServerFixtureCollection))]
public class WhenGettingWeeklyAviability
{
    private readonly TestServerFixture Given;
    private readonly DateTime SomeMonday = new(2024, 07, 08);

    public WhenGettingWeeklyAviability(TestServerFixture given)
    {
        Given = given ?? throw new Exception("Null Server");
    }

    [Fact]
    [ResetApplicationState]
    public async Task ReturnsResponseForMondaySingleDayOfWorkingSchedule()
    {
        var apiResponse = new AvaibilityWeeklyScheduleResponseBuilder()
            .WithFacility(facility => facility
                .WithFacilityId(Guid.NewGuid())
                .WithName("Las Palmeras")
                .WithAddress("Plaza de la independencia 36, 38006 Santa Cruz de Tenerife")
            )
            .WithSlotDurationMinutes(30)
            .WithMonday(monday => monday
                .WithWorkPeriod(period => period
                    .WithStartHour(9)
                    .WithLunchStartHour(10)
                    .WithLunchEndHour(11)
                    .WithEndHour(12)
                )
                .WithBusySlots(slots => slots
                    .WithBusySlot(slot => slot
                        .WithStart(SomeMonday.At(11,00))
                        .WithEnd(SomeMonday.At(11, 30))
                    )
                )
            )
            .Build();
        Given.AsssumeWeeklyScheduleReturnedByExternalApiForDate(SomeMonday, apiResponse);

        var url = GetAppointmentsUrl(SomeMonday);
        var response = await Given.Server.CreateClient().GetAsync<GetWeeklyAviabilityApiResponse>(url);

        response.Should().NotBeNull();

        response.Facility.Should().NotBeNull();
        
        response.Facility.FacilityId.Should().Be(apiResponse.Facility.FacilityId);
        response.Facility.Name.Should().Be(apiResponse.Facility.Name);
        response.Facility.Address.Should().Be(apiResponse.Facility.Address);

        response.SlotDurationMinutes.Should().Be(apiResponse.SlotDurationMinutes);

        response.Days.Should().NotBeNull();

        response.Days.Tuesday.Should().BeNull();
        response.Days.Wednesday.Should().BeNull();
        response.Days.Thursday.Should().BeNull();
        response.Days.Friday.Should().BeNull();

        response.Days.Monday.Should().NotBeNull();
        response.Days.Monday!.FreeSlots.Should().NotBeNullOrEmpty().And.HaveCount(3);

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
        thirdSlot.Start.Should().Be(SomeMonday.At(11, 30));
        thirdSlot.End.Should().Be(SomeMonday.At(12, 00));
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



