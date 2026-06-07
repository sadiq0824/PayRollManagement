using Microsoft.EntityFrameworkCore;
using PayrollManagement.API.Models;

namespace PayrollManagement.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Attendance> Attendance => Set<Attendance>();
    public DbSet<PayrollRun> PayrollRuns => Set<PayrollRun>();
    public DbSet<PayrollRunDetail> PayrollRunDetails => Set<PayrollRunDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Everything else (column types, lengths, required fields, unique indexes,
        // keys and foreign keys) is configured via Data Annotations directly on the
        // entity classes in /Models - see [Required], [MaxLength], [Column(TypeName=..)],
        // [Index], [Key] and [ForeignKey] attributes there.
        //
        // Delete behavior is the ONE thing Data Annotations cannot express, so it's
        // configured here. By default EF Core would set "Cascade" on these required
        // relationships - meaning deleting a Department would silently delete its
        // Employees, and deleting an Employee would silently wipe their payroll
        // history. For HR/financial data that is never the right default, so we
        // explicitly set Restrict: SQL Server will reject the delete instead.
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Employee)
            .WithMany(e => e.AttendanceRecords)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PayrollRunDetail>()
            .HasOne(d => d.PayrollRun)
            .WithMany(r => r.PayrollRunDetails)
            .HasForeignKey(d => d.PayrollRunId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PayrollRunDetail>()
            .HasOne(d => d.Employee)
            .WithMany(e => e.PayrollRunDetails)
            .HasForeignKey(d => d.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
