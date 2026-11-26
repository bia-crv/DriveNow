using DriveNow.Models;

namespace DriveNow.Services;

public class FleetStateService
{
    private readonly CarService _carService;
    private readonly BookingService _bookingService;
    private readonly LocationService _locationService;

    public FleetStateService(CarService carService, BookingService bookingService, LocationService locationService)
    {
        _carService = carService;
        _bookingService = bookingService;
        _locationService = locationService;
    }

    public List<Car> Cars { get; private set; } = new();
    public List<Booking> Bookings { get; private set; } = new();
    public List<Location> Locations { get; private set; } = new();

    public bool IsLoaded { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime? LastUpdated { get; private set; }

    public event Action? OnChange;

    public async Task LoadAsync(bool force = false)
    {
        if (IsLoaded && !force) return;

        var carsTask = _carService.GetAllAsync();
        var bookingsTask = _bookingService.GetAllAsync();
        var locationsTask = _locationService.GetAllAsync();

        await Task.WhenAll(carsTask, bookingsTask, locationsTask);

        var errors = new List<string>();

        if (carsTask.Result.Success) Cars = carsTask.Result.Data ?? new();
        else errors.Add($"Cars: {carsTask.Result.ErrorMessage}");

        if (bookingsTask.Result.Success) Bookings = bookingsTask.Result.Data ?? new();
        else errors.Add($"Bookings: {bookingsTask.Result.ErrorMessage}");

        if (locationsTask.Result.Success) Locations = locationsTask.Result.Data ?? new();
        else errors.Add($"Locations: {locationsTask.Result.ErrorMessage}");

        ErrorMessage = errors.Count > 0 ? string.Join(" ", errors) : null;
        IsLoaded = true;
        LastUpdated = DateTime.Now;
        NotifyChanged();
    }

    public bool IsBookingActive(Booking booking)
    {
        var now = DateTime.Now;
        return booking.PickupDate <= now && now < booking.ReturnDate;
    }

    public bool IsCarAvailable(Car car) =>
        car.Available && !Bookings.Any(b => b.CarId == car.Id && IsBookingActive(b));

    public int AvailableCarsCount => Cars.Count(IsCarAvailable);

    public int ActiveBookingsCount => Bookings.Count(IsBookingActive);

    public Car? FindCar(int id) => Cars.FirstOrDefault(c => c.Id == id);

    public Location? FindLocation(int id) => Locations.FirstOrDefault(l => l.Id == id);

    public void ClearError()
    {
        ErrorMessage = null;
        NotifyChanged();
    }

    public void NotifyChanged() => OnChange?.Invoke();
}