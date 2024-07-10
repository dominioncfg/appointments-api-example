using AppointmentsApi.Domain.Services;
using FluentAssertions.Extensions;

namespace AppointmentsApi.IntegrationTests.Common;
public class AvaibilityWeeklyScheduleResponseBuilder
{
    private FacilityResponse _facility = new();
    private int _slotDurationMinutes;
    private DayScheduleResponse? _monday;
    private DayScheduleResponse? _tuesday;
    private DayScheduleResponse? _wednesday;
    private DayScheduleResponse? _thursday;
    private DayScheduleResponse? _friday;
    private DayScheduleResponse? _saturday;
    private DayScheduleResponse? _sunday;

    public AvaibilityWeeklyScheduleResponseBuilder WithFacility(FacilityResponse facility)
    {
        _facility = facility;
        return this;
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithFacility(Action<FacilityResponseBuilder> facilityBuilder)
    {
        var builder = new FacilityResponseBuilder();
        facilityBuilder.Invoke(builder);
        return WithFacility(builder.Build());
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithSlotDurationMinutes(int slotDurationMinutes)
    {
        _slotDurationMinutes = slotDurationMinutes;
        return this;
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithMonday(DayScheduleResponse monday)
    {
        _monday = monday;
        return this;
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithMonday(Action<DayScheduleResponseBuilder> dayBuilder)
    {
        var builder = new DayScheduleResponseBuilder();
        dayBuilder.Invoke(builder);
        return WithMonday(builder.Build());
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithTuesday(DayScheduleResponse tuesday)
    {
        _tuesday = tuesday;
        return this;
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithTuesday(Action<DayScheduleResponseBuilder> dayBuilder)
    {
        var builder = new DayScheduleResponseBuilder();
        dayBuilder.Invoke(builder);
        return WithTuesday(builder.Build());
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithWednesday(DayScheduleResponse wednesday)
    {
        _wednesday = wednesday;
        return this;
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithWednesday(Action<DayScheduleResponseBuilder> dayBuilder)
    {
        var builder = new DayScheduleResponseBuilder();
        dayBuilder.Invoke(builder);
        return WithWednesday(builder.Build());
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithThursday(DayScheduleResponse thursday)
    {
        _thursday = thursday;
        return this;
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithThursday(Action<DayScheduleResponseBuilder> dayBuilder)
    {
        var builder = new DayScheduleResponseBuilder();
        dayBuilder.Invoke(builder);
        return WithThursday(builder.Build());
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithFriday(DayScheduleResponse friday)
    {
        _friday = friday;
        return this;
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithFriday(Action<DayScheduleResponseBuilder> dayBuilder)
    {
        var builder = new DayScheduleResponseBuilder();
        dayBuilder.Invoke(builder);
        return WithFriday(builder.Build());
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithSaturday(DayScheduleResponse saturday)
    {
        _saturday = saturday;
        return this;
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithSaturday(Action<DayScheduleResponseBuilder> dayBuilder)
    {
        var builder = new DayScheduleResponseBuilder();
        dayBuilder.Invoke(builder);
        return WithSaturday(builder.Build());
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithSunday(DayScheduleResponse sunday)
    {
        _sunday = sunday;
        return this;
    }

    public AvaibilityWeeklyScheduleResponseBuilder WithSunday(Action<DayScheduleResponseBuilder> dayBuilder)
    {
        var builder = new DayScheduleResponseBuilder();
        dayBuilder.Invoke(builder);
        return WithSunday(builder.Build());
    }

    public AvaibilityWeeklyScheduleResponse Build()
    {
        return new AvaibilityWeeklyScheduleResponse
        {
            Facility = _facility,
            SlotDurationMinutes = _slotDurationMinutes,
            Monday = _monday,
            Tuesday = _tuesday,
            Wednesday = _wednesday,
            Thursday = _thursday,
            Friday = _friday,
            Saturday = _saturday,
            Sunday = _sunday,
        };
    }

    private static readonly DateTime SomeMonday = new(2024, 07, 08);
    public static AvaibilityWeeklyScheduleResponseBuilder ValidWorkingScheduleResponse() => new AvaibilityWeeklyScheduleResponseBuilder()
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
                        .WithStart(SomeMonday.At(11, 00))
                        .WithEnd(SomeMonday.At(11, 30))
                    )
                )
            );

    public static AvaibilityWeeklyScheduleResponseBuilder ValidWorkingScheduleOnDayWithNoBusySlotsResponse() => new AvaibilityWeeklyScheduleResponseBuilder()
        .WithFacility(facility => facility
               .WithFacilityId(Guid.NewGuid())
               .WithName("Las Palmeras")
               .WithAddress("Plaza de la independencia 36, 38006 Santa Cruz de Tenerife")
           )
           .WithSlotDurationMinutes(30)
           .WithMonday(monday => monday
               .WithWorkPeriod(period => period
                   .WithStartHour(9)
                   .WithLunchStartHour(12)
                   .WithLunchEndHour(14)
                   .WithEndHour(17)
               )
           )
          .WithTuesday(day => day
                .WithWorkPeriod(period => period
                    .WithStartHour(9)
                    .WithLunchStartHour(12)
                    .WithLunchEndHour(14)
                    .WithEndHour(17)
                )
          )
          .WithWednesday(day => day
                .WithWorkPeriod(period => period
                    .WithStartHour(9)
                    .WithLunchStartHour(12)
                    .WithLunchEndHour(14)
                    .WithEndHour(17)
                )
          )
          .WithThursday(day => day
                  .WithWorkPeriod(period => period
                      .WithStartHour(9)
                      .WithLunchStartHour(12)
                      .WithLunchEndHour(14)
                      .WithEndHour(17)
                  )
          )
          .WithFriday(day => day
                  .WithWorkPeriod(period => period
                      .WithStartHour(9)
                      .WithLunchStartHour(12)
                      .WithLunchEndHour(14)
                      .WithEndHour(17)
                  )
          )
          .WithSaturday(day => day
                  .WithWorkPeriod(period => period
                      .WithStartHour(9)
                      .WithLunchStartHour(12)
                      .WithLunchEndHour(14)
                      .WithEndHour(17)
                  )
          )
          .WithSunday(day => day
                  .WithWorkPeriod(period => period
                      .WithStartHour(9)
                      .WithLunchStartHour(12)
                      .WithLunchEndHour(14)
                      .WithEndHour(17)
                  )
          );
}

public class FacilityResponseBuilder
{
    private Guid _facilityId;
    private string _name = string.Empty;
    private string _address = string.Empty;

    public FacilityResponseBuilder WithFacilityId(Guid facilityId)
    {
        _facilityId = facilityId;
        return this;
    }

    public FacilityResponseBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public FacilityResponseBuilder WithAddress(string address)
    {
        _address = address;
        return this;
    }

    public FacilityResponse Build()
    {
        return new FacilityResponse
        {
            FacilityId = _facilityId,
            Name = _name,
            Address = _address
        };
    }
}

public class WorkPeriodResponseBuilder
{
    private int _startHour;
    private int _endHour;
    private int _lunchStartHour;
    private int _lunchEndHour;

    public WorkPeriodResponseBuilder WithStartHour(int startHour)
    {
        _startHour = startHour;
        return this;
    }

    public WorkPeriodResponseBuilder WithEndHour(int endHour)
    {
        _endHour = endHour;
        return this;
    }

    public WorkPeriodResponseBuilder WithLunchStartHour(int lunchStartHour)
    {
        _lunchStartHour = lunchStartHour;
        return this;
    }

    public WorkPeriodResponseBuilder WithLunchEndHour(int lunchEndHour)
    {
        _lunchEndHour = lunchEndHour;
        return this;
    }

    public WorkPeriodResponse Build()
    {
        return new WorkPeriodResponse
        {
            StartHour = _startHour,
            EndHour = _endHour,
            LunchStartHour = _lunchStartHour,
            LunchEndHour = _lunchEndHour
        };
    }
}

public class BusySlotResponseBuilder
{
    private DateTime _start;
    private DateTime _end;

    public BusySlotResponseBuilder WithStart(DateTime start)
    {
        _start = start;
        return this;
    }

    public BusySlotResponseBuilder WithEnd(DateTime end)
    {
        _end = end;
        return this;
    }

    public BusySlotResponse Build()
    {
        return new BusySlotResponse
        {
            Start = _start,
            End = _end
        };
    }
}

public class DayScheduleResponseBuilder
{
    private WorkPeriodResponse _workPeriod = new();
    private readonly List<BusySlotResponse> _busySlots = [];

    public DayScheduleResponseBuilder WithWorkPeriod(WorkPeriodResponse workPeriod)
    {
        _workPeriod = workPeriod;
        return this;
    }

    public DayScheduleResponseBuilder WithWorkPeriod(Action<WorkPeriodResponseBuilder> periodBuilder)
    {
        var builder = new WorkPeriodResponseBuilder();
        periodBuilder.Invoke(builder);
        return WithWorkPeriod(builder.Build());
    }


    public DayScheduleResponseBuilder WithBusySlots(Action<BusySlotsCollectionResponseBuilder> busySlotsBuilder)
    {
        var builder = new BusySlotsCollectionResponseBuilder();
        busySlotsBuilder.Invoke(builder);
        _busySlots.AddRange(builder.Build());
        return this;
    }

    public DayScheduleResponse Build()
    {
        return new DayScheduleResponse
        {
            WorkPeriod = _workPeriod,
            BusySlots = _busySlots
        };
    }
}

public class BusySlotsCollectionResponseBuilder
{
    private readonly List<BusySlotResponse> _busySlots = [];

    public BusySlotsCollectionResponseBuilder WithBusySlot(Action<BusySlotResponseBuilder> slotBuilder)
    {
        var builder = new BusySlotResponseBuilder();
        slotBuilder.Invoke(builder);
        _busySlots.Add(builder.Build());
        return this;
    }

    public List<BusySlotResponse> Build() => _busySlots;
}