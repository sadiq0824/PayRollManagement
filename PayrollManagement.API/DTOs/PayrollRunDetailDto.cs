namespace PayrollManagement.API.DTOs;

public class PayrollRunDetailDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;

    public decimal BasicSalary { get; set; }
    public int WorkingDays { get; set; }
    public int DaysPresent { get; set; }

    public decimal GrossPay { get; set; }
    public decimal PFDeduction { get; set; }
    public decimal ProfessionalTax { get; set; }
    public decimal NetPay { get; set; }
}
