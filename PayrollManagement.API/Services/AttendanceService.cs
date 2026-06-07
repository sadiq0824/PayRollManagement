using PayrollManagement.API.DTOs;
using PayrollManagement.API.Mappings;
using PayrollManagement.API.Models;
using PayrollManagement.API.Repositories;

namespace PayrollManagement.API.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepository;

    public AttendanceService(IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    public async Task<AttendanceDto> AddAttendanceAsync(CreateAttendanceRequestDto request)
    {
        // Mirrors the unique index on Attendance(EmployeeId, Month, Year) -
        // checked here so the API can return a clean 409 Conflict before any write.
        if (await _attendanceRepository.ExistsAsync(request.EmployeeId, request.Month, request.Year))
        {
            throw new AttendanceAlreadyExistsException(request.EmployeeId, request.Month, request.Year);
        }

        var attendance = new Attendance
        {
            EmployeeId = request.EmployeeId,
            Month = request.Month,
            Year = request.Year,
            TotalWorkingDays = request.TotalWorkingDays,
            DaysPresent = request.DaysPresent,
        };

        var saved = await _attendanceRepository.AddAsync(attendance);
        return saved.ToDto();
    }
}
