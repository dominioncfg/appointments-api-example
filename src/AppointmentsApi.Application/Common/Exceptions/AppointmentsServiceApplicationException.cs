namespace AppointmentsApi.Application;

[Serializable]
public class AppointmentsServiceApplicationException : Exception
{
    public AppointmentsServiceApplicationException() { }
    public AppointmentsServiceApplicationException(string message) : base(message) { }
    public AppointmentsServiceApplicationException(string message, Exception inner) : base(message, inner) { }
    protected AppointmentsServiceApplicationException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
