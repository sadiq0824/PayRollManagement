const BASE_URL = import.meta.env.VITE_API_BASE_URL;

/**
 * Thin wrapper around fetch() that centralizes:
 *  - building the full URL from the configured API base
 *  - parsing JSON responses
 *  - turning non-2xx responses into thrown Errors with a readable message
 *
 * Every component calls these functions and only has to handle two cases:
 * "it worked" (returned data) or "it didn't" (caught error.message) -
 * they never touch fetch(), status codes, or JSON parsing directly.
 */
async function request(path, options = {}) {
  const response = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });

  // 204 No Content has no body to parse.
  const data = response.status === 204 ? null : await safeParseJson(response);

  if (!response.ok) {
    // The API returns { message: "..." } for 404/409, and a structured
    // { errors: { Field: ["..."] } } shape for 400 validation failures.
    // We normalize both into a single human-readable string here, so
    // every component can just display error.message.
    const message = extractErrorMessage(data) ?? `Request failed with status ${response.status}`;
    throw new Error(message);
  }

  return data;
}

async function safeParseJson(response) {
  try {
    return await response.json();
  } catch {
    return null;
  }
}

function extractErrorMessage(data) {
  if (!data) return null;
  if (typeof data.message === 'string') return data.message;

  if (data.errors) {
    const firstField = Object.keys(data.errors)[0];
    const firstMessage = data.errors[firstField]?.[0];
    if (firstMessage) return firstMessage;
  }

  return null;
}

export function getEmployees() {
  return request('/api/employees');
}

export function runPayroll(month, year) {
  return request('/api/payroll/run', {
    method: 'POST',
    body: JSON.stringify({ month, year }),
  });
}

export function getPayrollByMonthYear(month, year) {
  return request(`/api/payroll/${month}/${year}`);
}

export function getPayslip(runId, employeeId) {
  return request(`/api/payroll/${runId}/slip/${employeeId}`);
}

export function addAttendance({ employeeId, month, year, totalWorkingDays, daysPresent }) {
  return request('/api/attendance', {
    method: 'POST',
    body: JSON.stringify({ employeeId, month, year, totalWorkingDays, daysPresent }),
  });
}
