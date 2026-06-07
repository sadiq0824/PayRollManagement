import { useState } from 'react';
import AttendanceForm from './components/AttendanceForm';
import EmployeeList from './components/EmployeeList';
import PayrollPanel from './components/PayrollPanel';
import PayrollResultsTable from './components/PayrollResultsTable';
import PayslipModal from './components/PayslipModal';

export default function App() {
  // The payroll summary currently shown (result of "Run Payroll" or "View Payroll").
  const [summary, setSummary] = useState(null);

  // Which payslip the user asked to view, or null if the modal is closed.
  // Storing {runId, employeeId} (not the fetched payslip itself) lets
  // PayslipModal own its own fetch/loading/error lifecycle independently.
  const [payslipRequest, setPayslipRequest] = useState(null);

  return (
    <div className="container py-4">
      <header className="mb-4 no-print">
        <h1 className="h3 mb-1">Employee Payroll Run</h1>
        <p className="text-secondary mb-0">
          Generate, review, and inspect monthly payroll runs. Once generated, a payroll run is final and cannot be edited or deleted.
        </p>
      </header>

      <div className="d-flex flex-column gap-4 no-print">
        <AttendanceForm />

        <PayrollPanel onResult={setSummary} />

        <PayrollResultsTable
          summary={summary}
          onViewPayslip={(runId, employeeId) => setPayslipRequest({ runId, employeeId })}
        />

        <EmployeeList />
      </div>

      {payslipRequest && (
        <PayslipModal
          runId={payslipRequest.runId}
          employeeId={payslipRequest.employeeId}
          onClose={() => setPayslipRequest(null)}
        />
      )}
    </div>
  );
}
