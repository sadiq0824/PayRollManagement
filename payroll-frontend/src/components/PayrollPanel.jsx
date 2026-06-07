import { useState } from 'react';
import { runPayroll, getPayrollByMonthYear } from '../api/payrollApi';
import Spinner from './Spinner';
import ErrorAlert from './ErrorAlert';

const MONTHS = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December',
];

const currentYear = new Date().getFullYear();
const YEARS = Array.from({ length: 6 }, (_, i) => currentYear - 3 + i);

export default function PayrollPanel({ onResult }) {
  const [month, setMonth] = useState(new Date().getMonth() + 1);
  const [year, setYear] = useState(currentYear);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [info, setInfo] = useState(null);

  // Shared by both buttons - they only differ in which API call they make
  // and what message they show on success.
  async function handleAction(action, successMessage) {
    setLoading(true);
    setError(null);
    setInfo(null);
    try {
      const summary = await action(month, year);
      setInfo(successMessage);
      onResult(summary);
    } catch (err) {
      // The API distinguishes failure types with status codes (409 = already
      // exists, 404 = not found, 400 = invalid input) - payrollApi.js turns
      // all of them into err.message, and we just display it as-is. The
      // message itself ("Payroll for 5/2026 has already been generated...")
      // is already clear enough that the user doesn't need a status code.
      setError(err.message);
      onResult(null);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="card shadow-sm">
      <div className="card-header bg-white">
        <h2 className="h5 mb-0">Run / View Payroll</h2>
      </div>
      <div className="card-body">
        <div className="row g-3 align-items-end">
          <div className="col-sm-4 col-md-3">
            <label className="form-label" htmlFor="month-select">Month</label>
            <select
              id="month-select"
              className="form-select"
              value={month}
              onChange={(e) => setMonth(Number(e.target.value))}
              disabled={loading}
            >
              {MONTHS.map((name, index) => (
                <option key={name} value={index + 1}>{name}</option>
              ))}
            </select>
          </div>

          <div className="col-sm-4 col-md-3">
            <label className="form-label" htmlFor="year-select">Year</label>
            <select
              id="year-select"
              className="form-select"
              value={year}
              onChange={(e) => setYear(Number(e.target.value))}
              disabled={loading}
            >
              {YEARS.map((y) => (
                <option key={y} value={y}>{y}</option>
              ))}
            </select>
          </div>

          <div className="col-sm-4 col-md-6 d-flex gap-2">
            <button
              type="button"
              className="btn btn-primary"
              disabled={loading}
              onClick={() => handleAction(runPayroll, `Payroll generated for ${MONTHS[month - 1]} ${year}.`)}
            >
              Run Payroll
            </button>
            <button
              type="button"
              className="btn btn-outline-secondary"
              disabled={loading}
              onClick={() => handleAction(getPayrollByMonthYear, `Showing payroll for ${MONTHS[month - 1]} ${year}.`)}
            >
              View Payroll
            </button>
          </div>
        </div>

        <div className="mt-3">
          {loading && <Spinner label="Talking to the server..." />}
          <ErrorAlert message={error} onDismiss={() => setError(null)} />
          {info && !loading && !error && (
            <div className="alert alert-success mb-0" role="status">{info}</div>
          )}
        </div>
      </div>
    </div>
  );
}
