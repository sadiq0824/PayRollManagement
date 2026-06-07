# Employee Payroll Run Module

This is my submission for the Payroll Run Module assessment. It's a small
full-stack app that does what the brief describes: HR records employee
attendance for a month, runs payroll, and can pull up payslips afterwards —
basically replacing the spreadsheet workflow described in the brief.

Stack:
- **Backend** – ASP.NET Core 8 Web API (C#) talking to SQL Server through EF
  Core, with a Controller → Service → Repository → Stored Procedure layering
  (the actual payroll calculation happens in a stored procedure, the API just
  orchestrates around it).
- **Frontend** – React + Vite, styled with Bootstrap 5.
- **Database** – SQL Server (LocalDB is fine).

## Where things live

```
PayrollManagement.API/      the Web API
  SQLScripts/               01_CreateTables.sql, 02_SeedData.sql, 03_usp_RunPayroll.sql
  Data/Migrations/          EF Core migration, in case you'd rather use that than the raw scripts
PayrollManagement.Tests/    xUnit tests for the payroll math
payroll-frontend/           the React app
```

## Getting it running

### What you'll need
- .NET 8 SDK
- Node.js 18+ / npm
- A SQL Server instance (LocalDB, Express, whatever you've got)

### 1. Database

The connection string is in `PayrollManagement.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=PayrollManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Swap `Server=` for whatever you're running locally — `(localdb)\MSSQLLocalDB`,
`.\SQLEXPRESS`, a hostname, etc.

Then get the schema + data in. Two ways to do this, just pick one:

**Either** open the three scripts in `PayrollManagement.API/SQLScripts/` (in
SSMS, Azure Data Studio, sqlcmd, whatever you use) and run them against your
server in order:

1. `01_CreateTables.sql` – creates the database and all the tables/FKs/constraints
2. `02_SeedData.sql` – 3 departments, 5 employees, and attendance for the
   *current* month (it pulls the month/year from `GETDATE()` rather than a
   hardcoded value, so it lines up no matter when you run it — more on why
   below)
3. `03_usp_RunPayroll.sql` – creates the `usp_RunPayroll` stored procedure

**Or**, if you'd rather use EF migrations, from `PayrollManagement.API/`:

```bash
dotnet ef database update
```

That gets you the schema via the `InitialCreate` migration — but you'd still
need to run `02_SeedData.sql` and `03_usp_RunPayroll.sql` afterwards since the
seed data and stored procedure aren't part of the EF model.

### 2. Backend

```bash
cd PayrollManagement.API
dotnet run
```

Runs on `http://localhost:5235` (set in `Properties/launchSettings.json`).
Swagger's at `/swagger` if you want to poke the endpoints directly.

### 3. Frontend

```bash
cd payroll-frontend
cp .env.example .env
npm install
npm run dev
```

Vite will print the URL (usually `http://localhost:5173`). It reads the API's
address from `payroll-frontend/.env` (copied from `.env.example` above, since
`.env` itself is gitignored as it's machine-specific):

```
VITE_API_BASE_URL=http://localhost:5235
```

If your backend ends up on a different port, update that.

### 4. Tests

```bash
cd PayrollManagement.Tests
dotnet test
```

These cover `PayrollCalculator` — gross pay, PF, professional tax, net pay —
plus the edge cases I could think of: zero days present, zero working days
(division by zero), and net pay coming out negative when deductions outweigh
gross pay.

## Walking through the app

1. Open the frontend.
2. **Add Attendance** – pick an employee, month/year, working days and days
   present. The seed data already has the current month covered for all 5
   employees, so you mainly need this if you want to try a different period.
3. **Run Payroll** – choose month/year, hit "Run Payroll". This calls
   `POST /api/payroll/run`, which runs `usp_RunPayroll` under the hood. It'll
   refuse to re-run a period that's already been generated (immutability) and
   refuses to generate an empty run if nobody has attendance for that period.
4. **View Payroll** – re-fetches an existing run via `GET /api/payroll/{month}/{year}`.
5. Click an employee in the results table to open their **payslip**
   (`GET /api/payroll/{runId}/slip/{employeeId}`) — there's a Print button on
   it that produces a clean, print-only layout.

## API endpoints

| Method | Endpoint | What it does |
|---|---|---|
| GET | `/api/employees` | list all employees |
| POST | `/api/payroll/run` | run payroll for `{ month, year }` — 201 Created, or 409 if that period's already been run |
| GET | `/api/payroll/{month}/{year}` | fetch a saved run — 200 or 404 |
| GET | `/api/payroll/{runId}/slip/{employeeId}` | one employee's payslip — 200 or 404 |
| POST | `/api/attendance` | record attendance for `{ employeeId, month, year, totalWorkingDays, daysPresent }` — 201, or 409 if already recorded |

## Assumptions / things I did differently than the brief

- **I added attendance entry (`POST /api/attendance` + the "Add Attendance"
  form).** The brief's required list doesn't mention it explicitly — it just
  says to seed attendance for the current month — but a payroll run is useless
  without attendance data, and without some way to add it you'd be stuck
  re-running the seed script every time you wanted to try a different month.
  Felt like an obvious gap to fill so the thing is actually usable end to end.
- **Immutability is checked in three places on purpose**: the service layer
  checks first and returns a clean 409 before touching the database, the
  stored procedure checks again (it's the authoritative guard — it'd catch a
  duplicate even if something called it directly, bypassing the API), and
  there's a unique index on `PayrollRuns(Month, Year)` as the last line of
  defence. That's defence in depth, not me forgetting I'd already added the
  check somewhere else.
- **The stored procedure won't generate an empty run.** If no active employee
  has an attendance record for the month/year you ask for, it raises an error
  rather than quietly creating a payroll run with zero people in it. Right now
  that error bubbles up to the client as a raw 500 (it's an unhandled
  `SqlException`) — not great, see below for what I'd do about it.
- **The payroll formulas exist in two places** — once in `usp_RunPayroll`
  (the path that actually runs) and again in `PayrollCalculator.cs`, a small
  pure C# class that mirrors the math purely so it can be unit tested without
  needing SQL Server spun up. I went back and forth on this — duplicating
  logic is usually a smell — but the formulas are short, fixed by the brief,
  and unlikely to change, so the risk of the two drifting apart felt low
  compared to the value of having fast, no-DB tests.
- **Seed data attendance is for "this month", computed at insert time**
  (`DATEPART(MONTH/YEAR, GETDATE())`) rather than a fixed month/year. I did
  this specifically because the brief says to seed "the current month" — a
  hardcoded date would only be correct on the day I wrote it, and would break
  for anyone running the script later (which is exactly what happened to me
  during testing — ran into "no attendance records found for this period"
  because the seed had last month's date baked in).

## What I'd do next if I had more time

- **Turn the stored procedure's business-rule errors into proper HTTP
  responses.** Right now "no attendance for this period" comes back as a
  generic 500 because it's just an unhandled `SqlException` with error number
  50000. I'd catch that specific error number in the repository/service and
  turn it into a 400/422 with a readable message instead.
- **Pagination on the payroll endpoint** — listed as a bonus in the brief.
  The catch is `GET /api/payroll/{month}/{year}` returns a single run, so
  pagination doesn't really apply to it as written. If the module grew to need
  this, I'd add something like `GET /api/payroll` that lists runs (paginated),
  which is where pagination would actually make sense.
- **Integration tests** against the real stored procedure (Testcontainers or
  a local SQL instance) — the unit tests cover the math, but the
  duplicate-guard and empty-run-guard logic in the SP itself is currently only
  checked by hand.
- **Auth.** There's none right now, which felt fine for the scope of this
  assessment, but obviously wouldn't fly for something touching real salary
  data.
