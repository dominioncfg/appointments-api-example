using System.Net.Mime;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AppointmentsApi.IntegrationTests.Seedwork;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions SerializationOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static async Task GetAndExpectNotFoundAsync(this HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public static async Task<T> GetAsync<T>(this HttpClient client, string url)
    {
        using var getResponse = await client.GetAsync(url);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getResponse.IsSuccessStatusCode.Should().BeTrue();
        var responseModel = await getResponse.DeserializeAsync<T>();
        responseModel.Should().NotBeNull();
        return responseModel;
    }

    public static async Task GetAndExpectBadRequestAsync(this HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public static async Task PostAndExpectBadRequestAsync(this HttpClient client, string url, object request)
    {
        var json = JsonSerializer.Serialize(request, SerializationOptions);
        var httpContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await client.PostAsync(url, httpContent);
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public static async Task PostAndExpectCreatedAsync(this HttpClient client, string url, object request)
    {
        var json = JsonSerializer.Serialize(request, SerializationOptions);
        var httpContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await client.PostAsync(url, httpContent);
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }   
}