namespace DriveNow.Services;

public class ServiceResult<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? ErrorMessage { get; init; }
    public int? StatusCode { get; init; }

    public static ServiceResult<T> Ok(T data) =>
        new() { Success = true, Data = data };

    public static ServiceResult<T> Fail(string message, int? statusCode = null) =>
        new() { Success = false, ErrorMessage = message, StatusCode = statusCode };
}