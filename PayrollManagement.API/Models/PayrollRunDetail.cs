using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PayrollManagement.API.Models;

// An employee can appear at most once per payroll run.
[Index(nameof(PayrollRunId), nameof(EmployeeId), IsUnique = true)]
public class PayrollRunDetail
{
    [Key]
    public int PayrollRunDetailId { get; set; }

    [Required]
    [ForeignKey(nameof(PayrollRun))]
    public int PayrollRunId { get; set; }

    [Required]
    [ForeignKey(nameof(Employee))]
    public int EmployeeId { get; set; }

    // Snapshot values - copied at calculation time so historical payslips never change,
    // even if the employee's salary or attendance is edited afterwards.
    [Column(TypeName = "decimal(18,2)")]
    public decimal BasicSalary { get; set; }

    public int WorkingDays { get; set; }

    public int DaysPresent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal GrossPay { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PFDeduction { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ProfessionalTax { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NetPay { get; set; }

    public PayrollRun? PayrollRun { get; set; }
    public Employee? Employee { get; set; }
}
