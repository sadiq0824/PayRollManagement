using PayrollManagement.API.DTOs;

namespace PayrollManagement.API.Services;

public interface IAttendanceService
{
    // Throws AttendanceAlreadyExistsException if this employee/month/year is already recorded.
    Task<AttendanceDto> AddAttendanceAsync(CreateAttendanceRequestDto request);
}
