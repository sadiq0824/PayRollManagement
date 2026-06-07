using Microsoft.AspNetCore.Mvc;
using PayrollManagement.API.DTOs;
using PayrollManagement.API.Services;

namespace PayrollManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    // Records attendance for a month/year. 409 if already recorded for that period.
    [HttpPost]
    [ProducesResponseType(typeof(AttendanceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AttendanceDto>> Create([FromBody] CreateAttendanceRequestDto request)
    {
        try
        {
            var attendance = await _attendanceService.AddAttendanceAsync(request);
            return StatusCode(StatusCodes.Status201Created, attendance);
        }
        catch (AttendanceAlreadyExistsException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
