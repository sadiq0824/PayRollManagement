namespace PayrollManagement.API.Services;

// Mirrors the calculation done by usp_RunPayroll, kept as plain C# so the
// formulas can be unit tested without spinning up SQL Server.
public static class PayrollCalculator
{
    public const decimal PFRate = 0.12m;
    public const decimal ProfessionalTax = 200m;

    public static decimal CalculateGrossPay(decimal basicSalary, int totalWorkingDays, int daysPresent)
    {
        if (totalWorkingDays <= 0)
            throw new ArgumentOutOfRangeException(nameof(totalWorkingDays), "Total working days must be greater than zero.");

        return Math.Round(basicSalary / totalWorkingDays * daysPresent, 2);
    }

    public static decimal CalculatePFDeduction(decimal basicSalary)
    {
        return Math.Round(basicSalary * PFRate, 2);
    }

    public static decimal CalculateNetPay(decimal grossPay, decimal pfDeduction, decimal professionalTax)
    {
        return Math.Round(grossPay - pfDeduction - professionalTax, 2);
    }
}
