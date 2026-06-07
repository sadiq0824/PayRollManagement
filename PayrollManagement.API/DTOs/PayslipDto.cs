namespace PayrollManagement.API.DTOs;

public class PayslipDto
{
    public int PayrollRunId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DateTime RunDate { get; set; }

    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;

    public decimal BasicSalary { get; set; }
    public int WorkingDays { get; set; }
    public int DaysPresent { get; set; }

    public decimal GrossPay { get; set; }
    public decimal PFDeduction { get; set; }
    public decimal ProfessionalTax { get; set; }
    public decimal NetPay { get; set; }
}
