using System.ComponentModel.DataAnnotations;

namespace DriveNow.Models;

public class Booking : IValidatableObject
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a car.")]
    public int CarId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a pickup location.")]
    public int LocationId { get; set; }

    [Required(ErrorMessage = "Customer name is required.")]
    [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
    public string CustomerName { get; set; } = string.Empty;

    public DateTime PickupDate { get; set; } = DateTime.Today;

    public DateTime ReturnDate { get; set; } = DateTime.Today.AddDays(1);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PickupDate.Date < DateTime.Today)
        {
            yield return new ValidationResult(
                "Pickup date cannot be in the past.",
                new[] { nameof(PickupDate) });
        }

        if (ReturnDate <= PickupDate)
        {
            yield return new ValidationResult(
                "Return date must be after the pickup date.",
                new[] { nameof(ReturnDate) });
        }
    }
}