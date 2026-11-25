using DriveNow.Models;

namespace DriveNow.Services;

public class LocationService : ApiServiceBase
{
    public LocationService(HttpClient http, ILogger<LocationService> logger) : base(http, logger) { }

    public Task<ServiceResult<List<Location>>> GetAllAsync() =>
        SendAsync<List<Location>>(() => Http.GetAsync("locations"));

    public Task<ServiceResult<Location>> GetByIdAsync(int id) =>
        SendAsync<Location>(() => Http.GetAsync($"locations/{id}"));
}