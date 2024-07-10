namespace AppointmentsApi.Domain;

public static class DateTimeExtensions
{
    public static DateTime GetStartOfSchedulingWeek(this DateTime targetDate)
    {
        DayOfWeek currentDay = targetDate.DayOfWeek;

        if (currentDay is DayOfWeek.Monday)
            return targetDate.Date;


        int daysToSubtract = currentDay is DayOfWeek.Sunday ? 6 : (int)currentDay - (int)DayOfWeek.Monday;

        DateTime previousMonday = targetDate.AddDays(-daysToSubtract);

        return previousMonday.Date;
    }
}
