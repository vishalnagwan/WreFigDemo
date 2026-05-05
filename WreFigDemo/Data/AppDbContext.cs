using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WreFigDemo.Identity;
using WreFigDemo.Models.Entities;

namespace WreFigDemo.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Region> Regions => Set<Region>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ScheduleEntry> ScheduleEntries => Set<ScheduleEntry>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<AppUserBranch> UserBranches => Set<AppUserBranch>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Composite PK for user-branch mapping
        builder.Entity<AppUserBranch>()
            .HasKey(ub => new { ub.UserId, ub.BranchId });

        builder.Entity<AppUserBranch>()
            .HasOne(ub => ub.User)
            .WithMany(u => u.UserBranches)
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<AppUserBranch>()
            .HasOne(ub => ub.Branch)
            .WithMany(b => b.UserBranches)
            .HasForeignKey(ub => ub.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        // One schedule entry per employee per day
        builder.Entity<ScheduleEntry>()
            .HasIndex(s => new { s.EmployeeId, s.Date })
            .IsUnique();

        builder.Entity<ScheduleEntry>()
            .Property(s => s.StatusCode)
            .HasMaxLength(10)
            .HasDefaultValue("—");

        builder.Entity<Branch>()
            .HasIndex(b => new { b.State, b.City });

        builder.Entity<AuditLog>()
            .HasIndex(a => a.Timestamp);
    }
}
