import { useEffect, useState } from 'react';
import { getPayslip } from '../api/payrollApi';
import Spinner from './Spinner';
import ErrorAlert from './ErrorAlert';

const currency = (value) =>
  Number(value).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

const MONTHS = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December',
];

/**
 * A lightweight, dependency-free "modal" - a fixed overlay plus a centered card.
 * (Bootstrap's JS-driven modal needs its bundle wired up via a ref/effect; for
 * one dialog, a plain conditional overlay is simpler to read and just as effective.)
 */
export default function PayslipModal({ runId, employeeId, onClose }) {
  const [payslip, setPayslip] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    let cancelled = false;

    async function load() {
      setLoading(true);
      setError(null);
      try {
        const data = await getPayslip(runId, employeeId);
        if (!cancelled) setPayslip(data);
      } catch (err) {
        if (!cancelled) setError(err.message);
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    load();
    return () => { cancelled = true; };
  }, [runId, employeeId]);

  return (
    <div
      className="payslip-overlay position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center"
      style={{ background: 'rgba(0,0,0,0.5)', zIndex: 1050 }}
      onClick={onClose}
    >
      <div
        id="payslip-printable"
        className="card shadow-lg"
        style={{ width: '100%', maxWidth: '480px' }}
        // Stop the click from bubbling to the overlay (which would close the modal).
        onClick={(e) => e.stopPropagation()}
      >
        <div className="card-header bg-white d-flex justify-content-between align-items-center">
          <h2 className="h5 mb-0">Payslip</h2>
          <div className="d-flex align-items-center gap-2 no-print">
            {payslip && !loading && !error && (
              <button type="button" className="btn btn-sm btn-outline-secondary" onClick={() => window.print()}>
                Print
              </button>
            )}
            <button type="button" className="btn-close" aria-label="Close" onClick={onClose} />
          </div>
        </div>

        <div className="card-body">
          {loading && <Spinner label="Loading payslip..." />}
          <ErrorAlert message={error} />

          {payslip && !loading && !error && (
            <>
              <div className="mb-3">
                <div className="fw-semibold fs-5">{payslip.employeeName}</div>
                <div className="text-secondary">{payslip.employeeCode} &middot; {payslip.departmentName}</div>
                <div className="text-secondary small">
                  Pay period: {MONTHS[payslip.month - 1]} {payslip.year} &middot; Run #{payslip.payrollRunId}
                </div>
              </div>

              <table className="table table-sm mb-0">
                <tbody>
                  <tr><td>Basic Salary</td><td className="text-end">₹{currency(payslip.basicSalary)}</td></tr>
                  <tr><td>Working Days</td><td className="text-end">{payslip.workingDays}</td></tr>
                  <tr><td>Days Present</td><td className="text-end">{payslip.daysPresent}</td></tr>
                  <tr className="table-light"><td className="fw-semibold">Gross Pay</td><td className="text-end fw-semibold">₹{currency(payslip.grossPay)}</td></tr>
                  <tr><td>PF Deduction (12%)</td><td className="text-end text-danger">&minus; ₹{currency(payslip.pfDeduction)}</td></tr>
                  <tr><td>Professional Tax</td><td className="text-end text-danger">&minus; ₹{currency(payslip.professionalTax)}</td></tr>
                  <tr className="table-success"><td className="fw-bold">Net Pay</td><td className="text-end fw-bold">₹{currency(payslip.netPay)}</td></tr>
                </tbody>
              </table>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
