using System.ComponentModel.DataAnnotations;

namespace DriveNow.Models;

public class Booking : IValidatableObject
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a car.")]
    public int CarId { get; set; }

    [Required(ErrorMessage = "Customer name is required.")]
    [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
    public string CustomerName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.Date < DateTime.Today)
        {
            yield return new ValidationResult(
                "Start date cannot be in the past.",
                new[] { nameof(StartDate) });
        }

        if (EndDate <= StartDate)
        {
            yield return new ValidationResult(
                "End date must be after the start date.",
                new[] { nameof(EndDate) });
        }
    }
}