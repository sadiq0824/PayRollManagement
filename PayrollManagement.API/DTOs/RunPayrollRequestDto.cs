using System.ComponentModel.DataAnnotations;

namespace PayrollManagement.API.DTOs;

public class RunPayrollRequestDto
{
    [Required]
    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
    public int Month { get; set; }

    [Required]
    [Range(2000, 2100, ErrorMessage = "Year must be a valid 4-digit year.")]
    public int Year { get; set; }
}
