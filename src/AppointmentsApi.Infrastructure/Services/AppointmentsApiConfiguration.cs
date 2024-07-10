namespace AppointmentsApi.Infrastructure.Services;

//TODO: This should be loaded from configuration
public static class AppointmentsApiConfiguration
{
    public static string ApiBaseUrl => "https://draliatest.azurewebsites.net/";
    public static string UserName => "techuser";
    public static string Password => "secretpassWord";
}
