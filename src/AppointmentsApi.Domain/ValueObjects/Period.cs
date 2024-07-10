namespace AppointmentsApi.Domain.ValueObjects;

public record Period
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }

    public Period(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public bool OverlapsWith(Period other)
    {
        return this.Start < other.End && this.End > other.Start;
    }

    public bool IsContainedWithin(Period other)
    {
        return this.Start >= other.Start && this.End <= other.End;
    }
}
