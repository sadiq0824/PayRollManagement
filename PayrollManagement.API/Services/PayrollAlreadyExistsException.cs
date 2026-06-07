namespace PayrollManagement.API.Services;

// Thrown when payroll for this month/year already ran; the controller
// catches it and returns 409 Conflict.
public class PayrollAlreadyExistsException : Exception
{
    public int Month { get; }
    public int Year { get; }

    public PayrollAlreadyExistsException(int month, int year)
        : base($"Payroll for {month}/{year} has already been generated and cannot be run again.")
    {
        Month = month;
        Year = year;
    }
}
