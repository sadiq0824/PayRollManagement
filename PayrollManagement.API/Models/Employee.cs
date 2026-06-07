using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PayrollManagement.API.Models;

[Index(nameof(EmployeeCode), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class Employee
{
    [Key]
    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(20)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Basic salary must be greater than zero.")]
    public decimal BasicSalary { get; set; }

    [Required]
    [ForeignKey(nameof(Department))]
    public int DepartmentId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public Department? Department { get; set; }
    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
    public ICollection<PayrollRunDetail> PayrollRunDetails { get; set; } = new List<PayrollRunDetail>();
}
