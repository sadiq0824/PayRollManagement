using PayrollManagement.API.Models;

namespace PayrollManagement.API.Repositories;

public interface IPayrollRepository
{
    Task<bool> ExistsAsync(int month, int year);

    // Includes detail rows and employee info.
    Task<PayrollRun?> GetByMonthYearAsync(int month, int year);

    Task<PayrollRunDetail?> GetPayslipAsync(int payrollRunId, int employeeId);

    // Calls the usp_RunPayroll stored procedure and returns the resulting run with its detail rows.
    Task<PayrollRun?> ExecuteRunPayrollAsync(int month, int year);
}
