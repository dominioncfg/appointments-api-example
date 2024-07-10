using System.Text.Json;

namespace AppointmentsApi.IntegrationTests.Seedwork;

public static class ResponseExtensions
{
    private static readonly JsonSerializerOptions SerializationOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async static Task<T> DeserializeAsync<T>(this HttpResponseMessage response)
    {
        string content = await response.Content.ReadAsStringAsync();
       
        var result = JsonSerializer.Deserialize<T>(content, SerializationOptions);

        return result is null ? throw new System.Exception("The Response could not be parsed.") : result;
    }
}
