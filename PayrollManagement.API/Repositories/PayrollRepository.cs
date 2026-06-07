using Microsoft.EntityFrameworkCore;
using PayrollManagement.API.Data;
using PayrollManagement.API.Models;

namespace PayrollManagement.API.Repositories;

public class PayrollRepository : IPayrollRepository
{
    private readonly AppDbContext _context;

    public PayrollRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(int month, int year)
    {
        return await _context.PayrollRuns
            .AsNoTracking()
            .AnyAsync(p => p.Month == month && p.Year == year);
    }

    public async Task<PayrollRun?> GetByMonthYearAsync(int month, int year)
    {
        return await _context.PayrollRuns
            .Include(p => p.PayrollRunDetails)
                .ThenInclude(d => d.Employee)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Month == month && p.Year == year);
    }

    public async Task<PayrollRunDetail?> GetPayslipAsync(int payrollRunId, int employeeId)
    {
        return await _context.PayrollRunDetails
            .Include(d => d.Employee)
                .ThenInclude(e => e!.Department)
            .Include(d => d.PayrollRun)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.PayrollRunId == payrollRunId && d.EmployeeId == employeeId);
    }

    public async Task<PayrollRun?> ExecuteRunPayrollAsync(int month, int year)
    {
        // Calls the stored procedure, which validates for duplicates, calculates
        // pay for every active employee with attendance for the period, and
        // inserts the PayrollRuns + PayrollRunDetails rows in one transaction.
        // ExecuteSqlInterpolatedAsync parameterizes @Month/@Year - safe from SQL injection.
        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"EXEC usp_RunPayroll @Month = {month}, @Year = {year}");

        // The stored procedure performs the insert; we read back the result via EF Core
        // so the API can return a fully-shaped object (with Employee navigation data)
        // without needing to map raw stored-procedure result sets.
        return await GetByMonthYearAsync(month, year);
    }
}
