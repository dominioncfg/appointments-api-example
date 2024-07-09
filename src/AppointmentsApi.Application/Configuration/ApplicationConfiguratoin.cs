using System.Reflection;

namespace AppointmentsApi.Application;

public class ApplicationConfiguration
{
    public static Assembly Assembly => typeof(ApplicationConfiguration).Assembly;
}
