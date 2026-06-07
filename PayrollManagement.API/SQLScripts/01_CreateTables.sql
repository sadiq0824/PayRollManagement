-- =====================================================================
-- PayrollManagement Database - Table Creation Script
-- Run this against an empty database, e.g.:
--     CREATE DATABASE PayrollManagementDb;
--     GO
--     USE PayrollManagementDb;
-- =====================================================================

-- ---------------------------------------------------------------------
-- 1. Departments
-- Lookup table: holds the list of departments employees belong to.
-- ---------------------------------------------------------------------
CREATE TABLE Departments
(
    DepartmentId    INT IDENTITY(1,1)   NOT NULL,
    DepartmentName  NVARCHAR(100)       NOT NULL,

    CONSTRAINT PK_Departments PRIMARY KEY (DepartmentId),
    CONSTRAINT UQ_Departments_DepartmentName UNIQUE (DepartmentName)
);
GO

-- ---------------------------------------------------------------------
-- 2. Employees
-- Master record for each employee, including their base pay rate.
-- ---------------------------------------------------------------------
CREATE TABLE Employees
(
    EmployeeId      INT IDENTITY(1,1)   NOT NULL,
    EmployeeCode    NVARCHAR(20)        NOT NULL,
    FullName        NVARCHAR(100)       NOT NULL,
    Email           NVARCHAR(100)       NOT NULL,
    BasicSalary     DECIMAL(18,2)       NOT NULL,
    DepartmentId    INT                 NOT NULL,
    IsActive        BIT                 NOT NULL CONSTRAINT DF_Employees_IsActive DEFAULT (1),
    CreatedDate     DATETIME2           NOT NULL CONSTRAINT DF_Employees_CreatedDate DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_Employees PRIMARY KEY (EmployeeId),
    CONSTRAINT UQ_Employees_EmployeeCode UNIQUE (EmployeeCode),
    CONSTRAINT UQ_Employees_Email UNIQUE (Email),
    CONSTRAINT FK_Employees_Departments FOREIGN KEY (DepartmentId)
        REFERENCES Departments (DepartmentId),
    CONSTRAINT CK_Employees_BasicSalary_Positive CHECK (BasicSalary > 0)
);
GO

-- Speeds up "find all employees in department X" lookups and the FK join.
CREATE INDEX IX_Employees_DepartmentId ON Employees (DepartmentId);
GO

-- ---------------------------------------------------------------------
-- 3. Attendance
-- One row per employee, per month/year - the raw input to payroll.
-- ---------------------------------------------------------------------
CREATE TABLE Attendance
(
    AttendanceId        INT IDENTITY(1,1)   NOT NULL,
    EmployeeId          INT                 NOT NULL,
    Month               INT                 NOT NULL,
    Year                INT                 NOT NULL,
    TotalWorkingDays    INT                 NOT NULL,
    DaysPresent         INT                 NOT NULL,

    CONSTRAINT PK_Attendance PRIMARY KEY (AttendanceId),
    CONSTRAINT FK_Attendance_Employees FOREIGN KEY (EmployeeId)
        REFERENCES Employees (EmployeeId),

    -- Only one attendance record per employee per month/year.
    CONSTRAINT UQ_Attendance_Employee_Month_Year UNIQUE (EmployeeId, Month, Year),

    CONSTRAINT CK_Attendance_Month_Range CHECK (Month BETWEEN 1 AND 12),
    CONSTRAINT CK_Attendance_Year_Range CHECK (Year BETWEEN 2000 AND 2100),
    CONSTRAINT CK_Attendance_WorkingDays_Positive CHECK (TotalWorkingDays > 0),
    CONSTRAINT CK_Attendance_DaysPresent_Range CHECK (DaysPresent BETWEEN 0 AND TotalWorkingDays)
);
GO

CREATE INDEX IX_Attendance_EmployeeId ON Attendance (EmployeeId);
GO

-- ---------------------------------------------------------------------
-- 4. PayrollRuns
-- Represents a single "Run Payroll for Month/Year" execution.
-- ---------------------------------------------------------------------
CREATE TABLE PayrollRuns
(
    PayrollRunId    INT IDENTITY(1,1)   NOT NULL,
    Month           INT                 NOT NULL,
    Year            INT                 NOT NULL,
    RunDate         DATETIME2           NOT NULL CONSTRAINT DF_PayrollRuns_RunDate DEFAULT (SYSUTCDATETIME()),
    IsFinalized     BIT                 NOT NULL CONSTRAINT DF_PayrollRuns_IsFinalized DEFAULT (1),

    CONSTRAINT PK_PayrollRuns PRIMARY KEY (PayrollRunId),

    -- Enforces "no duplicate payroll for the same month/year" at the DB level
    -- (the stored procedure also checks this explicitly before inserting).
    CONSTRAINT UQ_PayrollRuns_Month_Year UNIQUE (Month, Year),

    CONSTRAINT CK_PayrollRuns_Month_Range CHECK (Month BETWEEN 1 AND 12),
    CONSTRAINT CK_PayrollRuns_Year_Range CHECK (Year BETWEEN 2000 AND 2100)
);
GO

-- ---------------------------------------------------------------------
-- 5. PayrollRunDetails
-- Frozen, per-employee snapshot of a payroll run's calculation.
-- ---------------------------------------------------------------------
CREATE TABLE PayrollRunDetails
(
    PayrollRunDetailId  INT IDENTITY(1,1)   NOT NULL,
    PayrollRunId        INT                 NOT NULL,
    EmployeeId          INT                 NOT NULL,

    -- Snapshot columns: copied at calculation time so historical payslips
    -- never change even if Employees/Attendance data changes later.
    BasicSalary         DECIMAL(18,2)       NOT NULL,
    WorkingDays         INT                 NOT NULL,
    DaysPresent         INT                 NOT NULL,

    -- Calculated columns (computed once by usp_RunPayroll and stored).
    GrossPay            DECIMAL(18,2)       NOT NULL,
    PFDeduction         DECIMAL(18,2)       NOT NULL,
    ProfessionalTax     DECIMAL(18,2)       NOT NULL,
    NetPay              DECIMAL(18,2)       NOT NULL,

    CONSTRAINT PK_PayrollRunDetails PRIMARY KEY (PayrollRunDetailId),
    CONSTRAINT FK_PayrollRunDetails_PayrollRuns FOREIGN KEY (PayrollRunId)
        REFERENCES PayrollRuns (PayrollRunId),
    CONSTRAINT FK_PayrollRunDetails_Employees FOREIGN KEY (EmployeeId)
        REFERENCES Employees (EmployeeId),

    -- An employee can only appear once per payroll run.
    CONSTRAINT UQ_PayrollRunDetails_Run_Employee UNIQUE (PayrollRunId, EmployeeId)
);
GO

CREATE INDEX IX_PayrollRunDetails_PayrollRunId ON PayrollRunDetails (PayrollRunId);
CREATE INDEX IX_PayrollRunDetails_EmployeeId   ON PayrollRunDetails (EmployeeId);
GO
