namespace PayrollManagement.API.DTOs;

public class PayrollRunSummaryDto
{
    public int PayrollRunId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DateTime RunDate { get; set; }
    public bool IsFinalized { get; set; }

    // Aggregate figures - computed once when building the DTO, saving the
    // frontend from having to sum the detail rows itself.
    public int TotalEmployees { get; set; }
    public decimal TotalGrossPay { get; set; }
    public decimal TotalNetPay { get; set; }

    public List<PayrollRunDetailDto> Details { get; set; } = new();
}
