using PayrollManagement.API.DTOs;
using PayrollManagement.API.Models;

namespace PayrollManagement.API.Mappings;

// Manual entity -> DTO conversions (no AutoMapper, model's small enough not to need it).
public static class MappingExtensions
{
    public static EmployeeDto ToDto(this Employee employee)
    {
        return new EmployeeDto
        {
            EmployeeId = employee.EmployeeId,
            EmployeeCode = employee.EmployeeCode,
            FullName = employee.FullName,
            Email = employee.Email,
            BasicSalary = employee.BasicSalary,
            DepartmentName = employee.Department?.DepartmentName ?? string.Empty,
            IsActive = employee.IsActive
        };
    }

    public static AttendanceDto ToDto(this Attendance attendance)
    {
        return new AttendanceDto
        {
            AttendanceId = attendance.AttendanceId,
            EmployeeId = attendance.EmployeeId,
            EmployeeCode = attendance.Employee?.EmployeeCode ?? string.Empty,
            EmployeeName = attendance.Employee?.FullName ?? string.Empty,
            Month = attendance.Month,
            Year = attendance.Year,
            TotalWorkingDays = attendance.TotalWorkingDays,
            DaysPresent = attendance.DaysPresent
        };
    }

    public static PayrollRunDetailDto ToDto(this PayrollRunDetail detail)
    {
        return new PayrollRunDetailDto
        {
            EmployeeId = detail.EmployeeId,
            EmployeeCode = detail.Employee?.EmployeeCode ?? string.Empty,
            EmployeeName = detail.Employee?.FullName ?? string.Empty,
            BasicSalary = detail.BasicSalary,
            WorkingDays = detail.WorkingDays,
            DaysPresent = detail.DaysPresent,
            GrossPay = detail.GrossPay,
            PFDeduction = detail.PFDeduction,
            ProfessionalTax = detail.ProfessionalTax,
            NetPay = detail.NetPay
        };
    }

    public static PayrollRunSummaryDto ToSummaryDto(this PayrollRun run)
    {
        var details = run.PayrollRunDetails.Select(d => d.ToDto()).ToList();

        return new PayrollRunSummaryDto
        {
            PayrollRunId = run.PayrollRunId,
            Month = run.Month,
            Year = run.Year,
            RunDate = run.RunDate,
            IsFinalized = run.IsFinalized,
            TotalEmployees = details.Count,
            TotalGrossPay = details.Sum(d => d.GrossPay),
            TotalNetPay = details.Sum(d => d.NetPay),
            Details = details
        };
    }

    public static PayslipDto ToPayslipDto(this PayrollRunDetail detail)
    {
        return new PayslipDto
        {
            PayrollRunId = detail.PayrollRunId,
            Month = detail.PayrollRun?.Month ?? 0,
            Year = detail.PayrollRun?.Year ?? 0,
            RunDate = detail.PayrollRun?.RunDate ?? default,
            EmployeeCode = detail.Employee?.EmployeeCode ?? string.Empty,
            EmployeeName = detail.Employee?.FullName ?? string.Empty,
            DepartmentName = detail.Employee?.Department?.DepartmentName ?? string.Empty,
            BasicSalary = detail.BasicSalary,
            WorkingDays = detail.WorkingDays,
            DaysPresent = detail.DaysPresent,
            GrossPay = detail.GrossPay,
            PFDeduction = detail.PFDeduction,
            ProfessionalTax = detail.ProfessionalTax,
            NetPay = detail.NetPay
        };
    }
}
