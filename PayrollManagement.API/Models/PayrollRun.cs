using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PayrollManagement.API.Models;

// A payroll run is unique per month/year - DB-level guarantee against duplicates.
[Index(nameof(Month), nameof(Year), IsUnique = true)]
public class PayrollRun
{
    [Key]
    public int PayrollRunId { get; set; }

    [Required]
    [Range(1, 12)]
    public int Month { get; set; }

    [Required]
    [Range(2000, 2100)]
    public int Year { get; set; }

    public DateTime RunDate { get; set; } = DateTime.UtcNow;

    // Payroll runs are immutable once created - this flag is the application's
    // signal that no edits/deletes are permitted on this run or its details.
    public bool IsFinalized { get; set; } = true;

    public ICollection<PayrollRunDetail> PayrollRunDetails { get; set; } = new List<PayrollRunDetail>();
}
