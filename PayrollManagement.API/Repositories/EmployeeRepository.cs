using Microsoft.EntityFrameworkCore;
using PayrollManagement.API.Data;
using PayrollManagement.API.Models;

namespace PayrollManagement.API.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .Include(e => e.Department)
            .AsNoTracking()
            .OrderBy(e => e.FullName)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int employeeId)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
    }
}
