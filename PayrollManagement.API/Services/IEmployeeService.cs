using PayrollManagement.API.DTOs;

namespace PayrollManagement.API.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
}
