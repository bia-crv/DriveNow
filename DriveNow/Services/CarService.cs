using System.Net.Http.Json;
using DriveNow.Models;

namespace DriveNow.Services;

public class CarService : ApiServiceBase
{
    private const string DuplicateRegMessage = "A car with this registration number already exists.";

    public CarService(HttpClient http, ILogger<CarService> logger) : base(http, logger) { }

    public Task<ServiceResult<List<Car>>> GetAllAsync() =>
        SendAsync<List<Car>>(() => Http.GetAsync("cars"));

    public Task<ServiceResult<Car>> GetByIdAsync(int id) =>
        SendAsync<Car>(() => Http.GetAsync($"cars/{id}"));

    public async Task<ServiceResult<Car>> CreateAsync(Car car)
    {
        if (await IsRegistrationTakenAsync(car.RegistrationNumber, null))
            return ServiceResult<Car>.Fail(DuplicateRegMessage, 409);

        return await SendAsync<Car>(() => Http.PostAsJsonAsync("cars", car), DuplicateRegMessage);
    }

    public async Task<ServiceResult<Car>> UpdateAsync(Car car)
    {
        if (await IsRegistrationTakenAsync(car.RegistrationNumber, car.Id))
            return ServiceResult<Car>.Fail(DuplicateRegMessage, 409);

        return await SendAsync<Car>(() => Http.PutAsJsonAsync($"cars/{car.Id}", car), DuplicateRegMessage);
    }

    public Task<ServiceResult<bool>> DeleteAsync(int id) =>
        SendAsync<bool>(() => Http.DeleteAsync($"cars/{id}"),
            "This car has active bookings and can't be deleted.");

    private async Task<bool> IsRegistrationTakenAsync(string registration, int? ignoreCarId)
    {
        var result = await GetAllAsync();
        if (!result.Success || result.Data is null) return false;

        return result.Data.Any(c =>
            c.Id != ignoreCarId &&
            string.Equals(c.RegistrationNumber.Trim(), registration.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}