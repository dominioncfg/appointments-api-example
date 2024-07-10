using AppointmentsApi.Domain.Services;
using AppointmentsApi.Domain.ValueObjects;

namespace AppointmentsApi.Domain;

public record DaySchedule
{
    public DateOnly Day { get; init; }
    public List<TimePeriod> FreeSlots { get; init; }

    private DaySchedule(List<TimePeriod> freeSlots, DateOnly day)
    {
        FreeSlots = freeSlots;
        Day = day;
    }

    public bool IsSpotAvailable(TimePeriod tartgetSlot)
    {
        return FreeSlots.Any(freeSlot => tartgetSlot.IsContainedWithin(freeSlot));
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

        return new DaySchedule(appointmentsSlots, DateOnly.FromDateTime(schedulingDate));
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