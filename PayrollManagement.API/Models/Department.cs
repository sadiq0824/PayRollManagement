using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PayrollManagement.API.Models;

[Index(nameof(DepartmentName), IsUnique = true)]
public class Department
{
    [Key]
    public int DepartmentId { get; set; }

    [Required]
    [MaxLength(100)]
    public string DepartmentName { get; set; } = string.Empty;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
