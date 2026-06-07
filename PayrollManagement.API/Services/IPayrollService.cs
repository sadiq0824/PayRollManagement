using PayrollManagement.API.DTOs;

namespace PayrollManagement.API.Services;

public interface IPayrollService
{
    // Throws PayrollAlreadyExistsException if this month/year was already run - runs are immutable.
    Task<PayrollRunSummaryDto> RunPayrollAsync(int month, int year);

    Task<PayrollRunSummaryDto?> GetPayrollByMonthYearAsync(int month, int year);

    Task<PayslipDto?> GetPayslipAsync(int payrollRunId, int employeeId);
}
