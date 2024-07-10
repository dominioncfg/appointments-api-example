using AppointmentsApi.Domain.Services;
using AppointmentsApi.Domain.ValueObjects;

namespace AppointmentsApi.Domain;

public class WeekScheduler
{
    public int SlotDurationMinutes { get; init; }
    public DateOnly MondayDate { get; init; }
    public DaySchedule? Monday { get; init; }
    public DaySchedule? Tuesday { get; init; }
    public DaySchedule? Wednesday { get; init; }
    public DaySchedule? Thursday { get; init; }
    public DaySchedule? Friday { get; init; }
    public DaySchedule? Saturday { get; init; }
    public DaySchedule? Sunday { get; init; }

    private List<DaySchedule?> AllDays() => [Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday];

    private WeekScheduler
     (
        DateOnly mondayDate,
        int slotDurationMinutes,
        DaySchedule? monday,
        DaySchedule? tuesday,
        DaySchedule? wednesday,
        DaySchedule? thursday,
        DaySchedule? friday,
        DaySchedule? saturday,
        DaySchedule? sunday
    )
    {
        SlotDurationMinutes = slotDurationMinutes;
        MondayDate = mondayDate;
        Monday = monday;
        Tuesday = tuesday;
        Wednesday = wednesday;
        Thursday = thursday;
        Friday = friday;
        Saturday = saturday;
        Sunday = sunday;
    }
    public static WeekScheduler FromBusySlots(DateTime mondayDate, AvaibilityWeeklyScheduleResponse response)
    {
        var monday = CreateScheduleForDay(response.Monday, mondayDate, response.SlotDurationMinutes);
        var tuesday = CreateScheduleForDay(response.Tuesday, mondayDate.AddDays(1), response.SlotDurationMinutes);
        var wednesday = CreateScheduleForDay(response.Wednesday, mondayDate.AddDays(2), response.SlotDurationMinutes);
        var thursday = CreateScheduleForDay(response.Thursday, mondayDate.AddDays(3), response.SlotDurationMinutes);
        var friday = CreateScheduleForDay(response.Friday, mondayDate.AddDays(4), response.SlotDurationMinutes);
        var saturday = CreateScheduleForDay(response.Saturday, mondayDate.AddDays(5), response.SlotDurationMinutes);
        var sunday = CreateScheduleForDay(response.Saturday, mondayDate.AddDays(6), response.SlotDurationMinutes);
        return new WeekScheduler(DateOnly.FromDateTime(mondayDate), response.SlotDurationMinutes, monday, tuesday, wednesday, thursday, friday, saturday, sunday);
    }

    private static DaySchedule? CreateScheduleForDay(DayScheduleResponse? day, DateTime schedulingDate, int slotDurationMinutes)
    {
        return day is not null ? DaySchedule.FromBusyScheduleResponse(day, schedulingDate, slotDurationMinutes) : null;
    }

    public bool IsSlotFree(TimePeriod tartgetSlot)
    {
        List<DaySchedule?> days = AllDays();
        var slotDay = DateOnly.FromDateTime(tartgetSlot.Start);
        var schedulingDay = days.FirstOrDefault(x => x is not null && x.Day == slotDay);

        if (schedulingDay is null)
            return false;

        var isAvialable = schedulingDay.IsSpotAvailable(tartgetSlot);

        return isAvialable;
    }    
}
