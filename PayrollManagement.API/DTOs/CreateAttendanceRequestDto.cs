using System.ComponentModel.DataAnnotations;

namespace PayrollManagement.API.DTOs;

public class CreateAttendanceRequestDto
{
    [Required]
    public int EmployeeId { get; set; }

    [Required]
    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
    public int Month { get; set; }

    [Required]
    [Range(2000, 2100, ErrorMessage = "Year must be a valid 4-digit year.")]
    public int Year { get; set; }

    [Required]
    [Range(1, 31, ErrorMessage = "Total working days must be between 1 and 31.")]
    public int TotalWorkingDays { get; set; }

    [Required]
    [Range(0, 31, ErrorMessage = "Days present must be between 0 and 31.")]
    public int DaysPresent { get; set; }
}
