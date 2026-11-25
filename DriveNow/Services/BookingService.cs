using System.Net.Http.Json;
using DriveNow.Models;

namespace DriveNow.Services;

public class BookingService : ApiServiceBase
{
    private const string OverlapMessage = "This car is already booked for the selected dates.";

    public BookingService(HttpClient http, ILogger<BookingService> logger) : base(http, logger) { }

    public Task<ServiceResult<List<Booking>>> GetAllAsync() =>
        SendAsync<List<Booking>>(() => Http.GetAsync("bookings"));

    public Task<ServiceResult<Booking>> GetByIdAsync(int id) =>
        SendAsync<Booking>(() => Http.GetAsync($"bookings/{id}"));

    public async Task<ServiceResult<Booking>> CreateAsync(Booking booking)
    {
        var existing = await GetAllAsync();
        if (existing.Success && existing.Data is not null)
        {
            var overlaps = existing.Data.Any(b =>
                b.CarId == booking.CarId &&
                booking.PickupDate < b.ReturnDate &&
                b.PickupDate < booking.ReturnDate);

            if (overlaps)
                return ServiceResult<Booking>.Fail(OverlapMessage, 409);
        }

        return await SendAsync<Booking>(() => Http.PostAsJsonAsync("bookings", booking), OverlapMessage);
    }

    public Task<ServiceResult<bool>> DeleteAsync(int id) =>
        SendAsync<bool>(() => Http.DeleteAsync($"bookings/{id}"));
}