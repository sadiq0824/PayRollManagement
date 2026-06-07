using PayrollManagement.API.DTOs;
using PayrollManagement.API.Mappings;
using PayrollManagement.API.Repositories;

namespace PayrollManagement.API.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();
        return employees.Select(e => e.ToDto());
    }
}
