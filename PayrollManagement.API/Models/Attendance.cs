using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PayrollManagement.API.Models;

// One attendance record per employee, per month/year - enforced as a composite unique index.
[Index(nameof(EmployeeId), nameof(Month), nameof(Year), IsUnique = true)]
public class Attendance
{
    [Key]
    public int AttendanceId { get; set; }

    [Required]
    [ForeignKey(nameof(Employee))]
    public int EmployeeId { get; set; }

    [Required]
    [Range(1, 12)]
    public int Month { get; set; }

    [Required]
    [Range(2000, 2100)]
    public int Year { get; set; }

    [Required]
    [Range(1, 31)]
    public int TotalWorkingDays { get; set; }

    [Required]
    [Range(0, 31)]
    public int DaysPresent { get; set; }

    public Employee? Employee { get; set; }
}
