using PayrollManagement.API.DTOs;
using PayrollManagement.API.Mappings;
using PayrollManagement.API.Repositories;

namespace PayrollManagement.API.Services;

public class PayrollService : IPayrollService
{
    private readonly IPayrollRepository _payrollRepository;

    public PayrollService(IPayrollRepository payrollRepository)
    {
        _payrollRepository = payrollRepository;
    }

    public async Task<PayrollRunSummaryDto> RunPayrollAsync(int month, int year)
    {
        // Business rule: payroll runs are immutable - once generated for a
        // month/year, it can never be regenerated, edited, or deleted.
        // We check here (in addition to the stored procedure's own check) so
        // the API can return a clean 409 Conflict before any write is attempted -
        // "defense in depth" across Service -> Stored Procedure -> DB constraint.
        if (await _payrollRepository.ExistsAsync(month, year))
        {
            throw new PayrollAlreadyExistsException(month, year);
        }

        var run = await _payrollRepository.ExecuteRunPayrollAsync(month, year);

        // The stored procedure guarantees a run is created when no duplicate exists,
        // so a null result here would indicate an unexpected failure inside the SP.
        if (run is null)
        {
            throw new InvalidOperationException(
                $"Payroll run for {month}/{year} could not be generated. The stored procedure did not return a result.");
        }

        return run.ToSummaryDto();
    }

    public async Task<PayrollRunSummaryDto?> GetPayrollByMonthYearAsync(int month, int year)
    {
        var run = await _payrollRepository.GetByMonthYearAsync(month, year);
        return run?.ToSummaryDto();
    }

    public async Task<PayslipDto?> GetPayslipAsync(int payrollRunId, int employeeId)
    {
        var detail = await _payrollRepository.GetPayslipAsync(payrollRunId, employeeId);
        return detail?.ToPayslipDto();
    }
}
