import { useEffect, useState } from 'react';
import { getEmployees } from '../api/payrollApi';
import Spinner from './Spinner';
import ErrorAlert from './ErrorAlert';

const currency = (value) =>
  Number(value).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

export default function EmployeeList() {
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Runs once when the component mounts - the standard "fetch on load" pattern.
  // The empty dependency array [] tells React "only run this effect once".
  useEffect(() => {
    let cancelled = false;

    async function load() {
      setLoading(true);
      setError(null);
      try {
        const data = await getEmployees();
        if (!cancelled) setEmployees(data);
      } catch (err) {
        if (!cancelled) setError(err.message);
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    load();
    // Cleanup: if the component unmounts before the fetch resolves, ignore the result -
    // prevents the classic "set state on an unmounted component" warning/bug.
    return () => { cancelled = true; };
  }, []);

  return (
    <div className="card shadow-sm">
      <div className="card-header bg-white">
        <h2 className="h5 mb-0">Employees</h2>
      </div>
      <div className="card-body">
        {loading && <Spinner label="Loading employees..." />}
        <ErrorAlert message={error} onDismiss={() => setError(null)} />

        {!loading && !error && (
          <div className="table-responsive">
            <table className="table table-sm table-hover align-middle mb-0">
              <thead>
                <tr>
                  <th>Code</th>
                  <th>Name</th>
                  <th>Department</th>
                  <th>Email</th>
                  <th className="text-end">Basic Salary</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {employees.map((emp) => (
                  <tr key={emp.employeeId}>
                    <td>{emp.employeeCode}</td>
                    <td>{emp.fullName}</td>
                    <td>{emp.departmentName}</td>
                    <td>{emp.email}</td>
                    <td className="text-end">₹{currency(emp.basicSalary)}</td>
                    <td>
                      <span className={`badge ${emp.isActive ? 'text-bg-success' : 'text-bg-secondary'}`}>
                        {emp.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
