using PayrollManagement.API.Models;

namespace PayrollManagement.API.Repositories;

public interface IAttendanceRepository
{
    Task<bool> ExistsAsync(int employeeId, int month, int year);

    // Saves the record and returns it with Employee loaded.
    Task<Attendance> AddAsync(Attendance attendance);
}
