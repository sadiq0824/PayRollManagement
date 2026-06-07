import { useEffect, useState } from 'react';
import { getEmployees, addAttendance } from '../api/payrollApi';
import Spinner from './Spinner';
import ErrorAlert from './ErrorAlert';

const MONTHS = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December',
];

const currentYear = new Date().getFullYear();
const YEARS = Array.from({ length: 6 }, (_, i) => currentYear - 3 + i);

const emptyForm = {
  employeeId: '',
  month: new Date().getMonth() + 1,
  year: currentYear,
  totalWorkingDays: '',
  daysPresent: '',
};

export default function AttendanceForm() {
  const [employees, setEmployees] = useState([]);
  const [employeesError, setEmployeesError] = useState(null);
  const [form, setForm] = useState(emptyForm);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);
  const [info, setInfo] = useState(null);

  // Load the employee list once, to populate the "Employee" dropdown.
  useEffect(() => {
    let cancelled = false;

    async function load() {
      try {
        const data = await getEmployees();
        if (!cancelled) setEmployees(data);
      } catch (err) {
        if (!cancelled) setEmployeesError(err.message);
      }
    }

    load();
    return () => { cancelled = true; };
  }, []);

  function updateField(field, value) {
    setForm((prev) => ({ ...prev, [field]: value }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    setInfo(null);

    try {
      const saved = await addAttendance({
        employeeId: Number(form.employeeId),
        month: Number(form.month),
        year: Number(form.year),
        totalWorkingDays: Number(form.totalWorkingDays),
        daysPresent: Number(form.daysPresent),
      });

      setInfo(
        `Attendance recorded for ${saved.employeeName} - ${MONTHS[saved.month - 1]} ${saved.year} ` +
        `(${saved.daysPresent}/${saved.totalWorkingDays} days present).`
      );
      // Keep the selected month/year (likely entering attendance for several
      // employees in the same period back-to-back); clear the per-employee fields.
      setForm((prev) => ({ ...prev, employeeId: '', totalWorkingDays: '', daysPresent: '' }));
    } catch (err) {
      // Mirrors PayrollPanel: the API's message ("Attendance for employee X in
      // M/Y has already been recorded.") is already clear enough to show as-is.
      setError(err.message);
    } finally {
      setSubmitting(false);
    }
  }

  const disabled = submitting || employees.length === 0;

  return (
    <div className="card shadow-sm">
      <div className="card-header bg-white">
        <h2 className="h5 mb-0">Add Attendance</h2>
      </div>
      <div className="card-body">
        <ErrorAlert message={employeesError} onDismiss={() => setEmployeesError(null)} />

        <form onSubmit={handleSubmit}>
          <div className="row g-3 align-items-end">
            <div className="col-sm-6 col-md-4">
              <label className="form-label" htmlFor="attendance-employee">Employee</label>
              <select
                id="attendance-employee"
                className="form-select"
                value={form.employeeId}
                onChange={(e) => updateField('employeeId', e.target.value)}
                disabled={disabled}
                required
              >
                <option value="" disabled>Select an employee...</option>
                {employees.map((emp) => (
                  <option key={emp.employeeId} value={emp.employeeId}>
                    {emp.employeeCode} - {emp.fullName}
                  </option>
                ))}
              </select>
            </div>

            <div className="col-sm-3 col-md-2">
              <label className="form-label" htmlFor="attendance-month">Month</label>
              <select
                id="attendance-month"
                className="form-select"
                value={form.month}
                onChange={(e) => updateField('month', e.target.value)}
                disabled={disabled}
              >
                {MONTHS.map((name, index) => (
                  <option key={name} value={index + 1}>{name}</option>
                ))}
              </select>
            </div>

            <div className="col-sm-3 col-md-2">
              <label className="form-label" htmlFor="attendance-year">Year</label>
              <select
                id="attendance-year"
                className="form-select"
                value={form.year}
                onChange={(e) => updateField('year', e.target.value)}
                disabled={disabled}
              >
                {YEARS.map((y) => (
                  <option key={y} value={y}>{y}</option>
                ))}
              </select>
            </div>

            <div className="col-sm-6 col-md-2">
              <label className="form-label" htmlFor="attendance-working-days">Working Days</label>
              <input
                id="attendance-working-days"
                type="number"
                className="form-control"
                min={1}
                max={31}
                value={form.totalWorkingDays}
                onChange={(e) => updateField('totalWorkingDays', e.target.value)}
                disabled={disabled}
                required
              />
            </div>

            <div className="col-sm-6 col-md-2">
              <label className="form-label" htmlFor="attendance-days-present">Days Present</label>
              <input
                id="attendance-days-present"
                type="number"
                className="form-control"
                min={0}
                max={31}
                value={form.daysPresent}
                onChange={(e) => updateField('daysPresent', e.target.value)}
                disabled={disabled}
                required
              />
            </div>

            <div className="col-12">
              <button type="submit" className="btn btn-primary" disabled={disabled}>
                {submitting ? 'Saving...' : 'Add Attendance'}
              </button>
            </div>
          </div>
        </form>

        <div className="mt-3">
          {submitting && <Spinner label="Saving attendance..." />}
          <ErrorAlert message={error} onDismiss={() => setError(null)} />
          {info && !submitting && !error && (
            <div className="alert alert-success mb-0" role="status">{info}</div>
          )}
        </div>
      </div>
    </div>
  );
}
