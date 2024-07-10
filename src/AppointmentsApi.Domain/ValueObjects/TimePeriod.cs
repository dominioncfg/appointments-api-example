namespace AppointmentsApi.Domain.ValueObjects;

public record TimePeriod
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }

    public TimePeriod(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public bool OverlapsWith(TimePeriod other)
    {
        return this.Start < other.End && this.End > other.Start;
    }

    public bool IsContainedWithin(TimePeriod other)
    {
        return this.Start >= other.Start && this.End <= other.End;
    }
}
