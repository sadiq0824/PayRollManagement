using PayrollManagement.API.Services;
using Xunit;

namespace PayrollManagement.Tests;

public class PayrollCalculatorTests
{
    [Theory]
    [InlineData(30000, 26, 24, 27692.31)]   // brief's worked example: Ravi Sharma
    [InlineData(60000, 22, 22, 60000.00)]   // full attendance - gross pay equals basic salary
    [InlineData(45000, 22, 0, 0.00)]        // edge case: zero days present -> zero gross pay
    public void CalculateGrossPay_ReturnsProRatedAmount_RoundedToTwoDecimals(
        decimal basicSalary, int totalWorkingDays, int daysPresent, decimal expected)
    {
        var result = PayrollCalculator.CalculateGrossPay(basicSalary, totalWorkingDays, daysPresent);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateGrossPay_ThrowsForZeroWorkingDays()
    {
        // Guards against a division by zero - a month with no working days
        // configured is bad data, not a payable period.
        Assert.Throws<ArgumentOutOfRangeException>(
            () => PayrollCalculator.CalculateGrossPay(30000, totalWorkingDays: 0, daysPresent: 0));
    }

    [Theory]
    [InlineData(30000, 3600.00)]
    [InlineData(60000, 7200.00)]
    [InlineData(45000.50, 5400.06)]   // exercises rounding to 2 decimals
    public void CalculatePFDeduction_Returns12PercentOfBasicSalary_RoundedToTwoDecimals(
        decimal basicSalary, decimal expected)
    {
        var result = PayrollCalculator.CalculatePFDeduction(basicSalary);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateNetPay_SubtractsDeductionsFromGrossPay()
    {
        // Brief's worked example: Gross 27,692 - PF 3,600 - Professional Tax 200 = Net 23,892
        var result = PayrollCalculator.CalculateNetPay(grossPay: 27692.31m, pfDeduction: 3600m, professionalTax: 200m);

        Assert.Equal(23892.31m, result);
    }

    [Fact]
    public void CalculateNetPay_CanBeNegative_WhenDeductionsExceedGrossPay()
    {
        // Edge case: an employee with very low attendance can have deductions
        // (PF is a flat % of basic salary, not of gross pay) exceed their gross pay.
        // The brief defines Net Pay as a straight subtraction, so a negative
        // result is the mathematically correct (if HR-noteworthy) outcome.
        var result = PayrollCalculator.CalculateNetPay(grossPay: 100m, pfDeduction: 3600m, professionalTax: 200m);

        Assert.Equal(-3700m, result);
    }
}
