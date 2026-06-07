using Microsoft.EntityFrameworkCore;
using PayrollManagement.API.Data;
using PayrollManagement.API.Models;

namespace PayrollManagement.API.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext _context;

    public AttendanceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(int employeeId, int month, int year)
    {
        return await _context.Attendance
            .AsNoTracking()
            .AnyAsync(a => a.EmployeeId == employeeId && a.Month == month && a.Year == year);
    }

    public async Task<Attendance> AddAsync(Attendance attendance)
    {
        _context.Attendance.Add(attendance);
        await _context.SaveChangesAsync();

        // Re-fetch with Employee (+ Department, used by the DTO mapper) loaded -
        // the entity we just inserted only has the FK, not the navigation.
        return await _context.Attendance
            .Include(a => a.Employee)
                .ThenInclude(e => e!.Department)
            .AsNoTracking()
            .FirstAsync(a => a.AttendanceId == attendance.AttendanceId);
    }
}
