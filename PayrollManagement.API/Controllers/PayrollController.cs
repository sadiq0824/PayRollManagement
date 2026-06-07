using Microsoft.AspNetCore.Mvc;
using PayrollManagement.API.DTOs;
using PayrollManagement.API.Services;

namespace PayrollManagement.API.Controllers;

[ApiController]
[Route("api/payroll")]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public PayrollController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    // Generates payroll for a month/year. 409 if it already exists - runs are immutable.
    [HttpPost("run")]
    [ProducesResponseType(typeof(PayrollRunSummaryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PayrollRunSummaryDto>> RunPayroll([FromBody] RunPayrollRequestDto request)
    {
        // [ApiController] automatically returns 400 Bad Request with a
        // validation-error body if RunPayrollRequestDto's [Range] attributes
        // fail - this method only runs once the DTO shape is already valid.
        try
        {
            var summary = await _payrollService.RunPayrollAsync(request.Month, request.Year);

            // 201 Created points the client at the resource it just created
            // (GET /api/payroll/{month}/{year}) and includes it in the body -
            // the standard REST shape for a successful creation.
            return CreatedAtAction(
                nameof(GetByMonthYear),
                new { month = summary.Month, year = summary.Year },
                summary);
        }
        catch (PayrollAlreadyExistsException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // Returns the payroll run for a given month/year, with employee detail rows.
    [HttpGet("{month:int}/{year:int}")]
    [ProducesResponseType(typeof(PayrollRunSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PayrollRunSummaryDto>> GetByMonthYear(int month, int year)
    {
        var summary = await _payrollService.GetPayrollByMonthYearAsync(month, year);

        if (summary is null)
        {
            return NotFound(new { message = $"No payroll run found for {month}/{year}." });
        }

        return Ok(summary);
    }

    // Returns one employee's payslip from a specific payroll run.
    [HttpGet("{runId:int}/slip/{employeeId:int}")]
    [ProducesResponseType(typeof(PayslipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PayslipDto>> GetPayslip(int runId, int employeeId)
    {
        var payslip = await _payrollService.GetPayslipAsync(runId, employeeId);

        if (payslip is null)
        {
            return NotFound(new { message = $"No payslip found for employee {employeeId} in payroll run {runId}." });
        }

        return Ok(payslip);
    }
}
