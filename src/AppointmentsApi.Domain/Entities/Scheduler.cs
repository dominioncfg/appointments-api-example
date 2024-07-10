using AppointmentsApi.Domain.Services;
using AppointmentsApi.Domain.ValueObjects;

namespace AppointmentsApi.Domain;

public class WeekSchedulerScheduler
{
    public int SlotDurationMinutes { get; init; }
    public DateTime MondayDate { get; init; }
    public DaySchedule? Monday { get; init; }
    public DaySchedule? Tuesday { get; init; }
    public DaySchedule? Wednesday { get; init; }
    public DaySchedule? Thursday { get; init; }
    public DaySchedule? Friday { get; init; }
    public DaySchedule? Saturday { get; init; }
    public DaySchedule? Sunday { get; init; }

    private WeekSchedulerScheduler
     (
        DateTime mondayDate,
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
    public static WeekSchedulerScheduler FromBusySlots(DateTime mondayDate, AvaibilityWeeklyScheduleResponse response)
    {
        var monday = CreateScheduleForDay(response.Monday, mondayDate, response.SlotDurationMinutes);
        var tuesday = CreateScheduleForDay(response.Tuesday, mondayDate.AddDays(1), response.SlotDurationMinutes);
        var wednesday = CreateScheduleForDay(response.Wednesday, mondayDate.AddDays(2), response.SlotDurationMinutes);
        var thursday = CreateScheduleForDay(response.Thursday, mondayDate.AddDays(3), response.SlotDurationMinutes);
        var friday = CreateScheduleForDay(response.Friday, mondayDate.AddDays(4), response.SlotDurationMinutes);
        var saturday = CreateScheduleForDay(response.Saturday, mondayDate.AddDays(5), response.SlotDurationMinutes);
        var sunday = CreateScheduleForDay(response.Saturday, mondayDate.AddDays(6), response.SlotDurationMinutes);
        return new WeekSchedulerScheduler(mondayDate,response.SlotDurationMinutes,monday,tuesday,wednesday,thursday,friday,saturday,sunday);
    }

    private static DaySchedule? CreateScheduleForDay(DayScheduleResponse? day, DateTime schedulingDate, int slotDurationMinutes)
    {
        return day is not null ? DaySchedule.FromBusyScheduleResponse(day, schedulingDate, slotDurationMinutes) : null;
    }
}

public record DaySchedule
{

    public List<TimePeriod> FreeSlots { get; init; }

    private DaySchedule(List<TimePeriod> freeSlots)
    {
        FreeSlots = freeSlots;
    }


    public static DaySchedule FromBusyScheduleResponse(DayScheduleResponse day, DateTime schedulingDate, int slotDurationMinutes)
    {
        var activeWorkingPeriods = GetActiveWorkingPeriods(day, schedulingDate);
        var busySlots = day.BusySlots.Select(x => new TimePeriod(x.Start, x.End)).ToList();

        var appointmentsSlots = new List<TimePeriod>();
        foreach (var workingPeriod in activeWorkingPeriods)
        {
            var periodStart = workingPeriod.Start;
            while (periodStart < workingPeriod.End)
            {
                var slotEnd = periodStart.AddMinutes(slotDurationMinutes);
                var appointmentSlot = new TimePeriod(periodStart, slotEnd);
                if (appointmentSlot.IsContainedWithin(workingPeriod) && !busySlots.Any(x => x.OverlapsWith(appointmentSlot)))
                {
                    appointmentsSlots.Add(appointmentSlot);
                }
                periodStart = slotEnd;
            }
        }

        return new DaySchedule(appointmentsSlots);
    }

    private static List<TimePeriod> GetActiveWorkingPeriods(DayScheduleResponse day, DateTime schedulingDate)
    {
        var dayStartingHour = schedulingDate.AddHours(day.WorkPeriod.StartHour);
        var dayFinishingHour = schedulingDate.AddHours(day.WorkPeriod.EndHour);

        var lunchStartingHour = schedulingDate.AddHours(day.WorkPeriod.LunchStartHour);
        var lunchFinishingHour = schedulingDate.AddHours(day.WorkPeriod.LunchEndHour);

        var workingPeriod = new TimePeriod(dayStartingHour, dayFinishingHour);
        var lunchPeriod = new TimePeriod(lunchStartingHour, lunchFinishingHour);


        if (!lunchPeriod.IsContainedWithin(workingPeriod))
            return [workingPeriod];


        return
        [
            new TimePeriod(dayStartingHour,lunchStartingHour),
            new TimePeriod(lunchFinishingHour,dayFinishingHour)
        ];
    }
}