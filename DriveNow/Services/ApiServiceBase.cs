using System.Net;
using System.Text.Json;

namespace DriveNow.Services;

public abstract class ApiServiceBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    protected readonly HttpClient Http;
    private readonly ILogger _logger;

    protected ApiServiceBase(HttpClient http, ILogger logger)
    {
        Http = http;
        _logger = logger;
    }

    protected async Task<ServiceResult<T>> SendAsync<T>(
        Func<Task<HttpResponseMessage>> request,
        string conflictMessage = "This action conflicts with existing data.")
    {
        try
        {
            using var response = await request();

            if (response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(bool))
                    return ServiceResult<T>.Ok((T)(object)true);

                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json))
                    return ServiceResult<T>.Ok(default!);

                var data = JsonSerializer.Deserialize<T>(json, JsonOptions);
                return ServiceResult<T>.Ok(data!);
            }

            var message = response.StatusCode switch
            {
                HttpStatusCode.Conflict => conflictMessage,
                HttpStatusCode.NotFound => "The item was not found. It may have been deleted.",
                HttpStatusCode.BadRequest => "The server rejected the data. Check the fields and try again.",
                _ => "The server returned an error. Try again later."
            };

            _logger.LogWarning("API returned {Status} for {Url}", (int)response.StatusCode, response.RequestMessage?.RequestUri);
            return ServiceResult<T>.Fail(message, (int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Could not reach the API");
            return ServiceResult<T>.Fail("Couldn't reach the DriveNow server. Check your connection and try again.");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "API request timed out");
            return ServiceResult<T>.Fail("The server took too long to respond. Try again.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Unexpected JSON from the API");
            return ServiceResult<T>.Fail("The server sent data in an unexpected format.");
        }
    }
}