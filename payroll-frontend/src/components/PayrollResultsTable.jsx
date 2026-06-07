const currency = (value) =>
  Number(value).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

export default function PayrollResultsTable({ summary, onViewPayslip }) {
  if (!summary) return null;

  return (
    <div className="card shadow-sm">
      <div className="card-header bg-white d-flex flex-wrap justify-content-between align-items-center gap-2">
        <h2 className="h5 mb-0">
          Payroll Run #{summary.payrollRunId} &mdash; {summary.month}/{summary.year}
        </h2>
        <span className={`badge ${summary.isFinalized ? 'text-bg-success' : 'text-bg-warning'}`}>
          {summary.isFinalized ? 'Finalized (immutable)' : 'Draft'}
        </span>
      </div>

      <div className="card-body">
        <div className="row text-center mb-3 g-3">
          <div className="col-sm-4">
            <div className="border rounded p-2">
              <div className="text-secondary small">Employees Paid</div>
              <div className="fs-4 fw-semibold">{summary.totalEmployees}</div>
            </div>
          </div>
          <div className="col-sm-4">
            <div className="border rounded p-2">
              <div className="text-secondary small">Total Gross Pay</div>
              <div className="fs-4 fw-semibold">₹{currency(summary.totalGrossPay)}</div>
            </div>
          </div>
          <div className="col-sm-4">
            <div className="border rounded p-2">
              <div className="text-secondary small">Total Net Pay</div>
              <div className="fs-4 fw-semibold">₹{currency(summary.totalNetPay)}</div>
            </div>
          </div>
        </div>

        <div className="table-responsive">
          <table className="table table-sm table-hover align-middle mb-0">
            <thead>
              <tr>
                <th>Code</th>
                <th>Employee</th>
                <th className="text-end">Basic Salary</th>
                <th className="text-end">Days</th>
                <th className="text-end">Gross Pay</th>
                <th className="text-end">PF</th>
                <th className="text-end">Tax</th>
                <th className="text-end">Net Pay</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {summary.details.map((row) => (
                <tr key={row.employeeId}>
                  <td>{row.employeeCode}</td>
                  <td>{row.employeeName}</td>
                  <td className="text-end">₹{currency(row.basicSalary)}</td>
                  <td className="text-end">{row.daysPresent} / {row.workingDays}</td>
                  <td className="text-end">₹{currency(row.grossPay)}</td>
                  <td className="text-end">₹{currency(row.pfDeduction)}</td>
                  <td className="text-end">₹{currency(row.professionalTax)}</td>
                  <td className="text-end fw-semibold">₹{currency(row.netPay)}</td>
                  <td className="text-end">
                    <button
                      type="button"
                      className="btn btn-sm btn-outline-primary"
                      onClick={() => onViewPayslip(summary.payrollRunId, row.employeeId)}
                    >
                      View Payslip
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
