using System.Text.Json;

namespace AppointmentsApi.IntegrationTests.Seedwork;

public static class ResponseExtensions
{
    public async static Task<T> DeserializeAsync<T>(this HttpResponseMessage response)
    {
        string content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var result = JsonSerializer.Deserialize<T>(content, options);

        return result is null ? throw new System.Exception("The Response could not be parsed.") : result;
    }
}
