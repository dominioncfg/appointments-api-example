namespace AppointmentsApi.Application;

[Serializable]
public class SlotIsNotFreeApplicationException : AppointmentsServiceApplicationException
{
    public SlotIsNotFreeApplicationException() { }
    public SlotIsNotFreeApplicationException(string message) : base(message) { }
    public SlotIsNotFreeApplicationException(string message, Exception inner) : base(message, inner) { }

}
