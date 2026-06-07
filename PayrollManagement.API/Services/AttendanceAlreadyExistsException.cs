namespace PayrollManagement.API.Services;

// Thrown when attendance for this employee/month/year is already recorded;
// the controller catches it and returns 409 Conflict.
public class AttendanceAlreadyExistsException : Exception
{
    public int EmployeeId { get; }
    public int Month { get; }
    public int Year { get; }

    public AttendanceAlreadyExistsException(int employeeId, int month, int year)
        : base($"Attendance for employee {employeeId} in {month}/{year} has already been recorded.")
    {
        EmployeeId = employeeId;
        Month = month;
        Year = year;
    }
}
