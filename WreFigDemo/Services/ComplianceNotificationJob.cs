using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WreFigDemo.Data;
using WreFigDemo.Identity;

namespace WreFigDemo.Services;

public class ComplianceNotificationJob(
    IServiceProvider services,
    IConfiguration config,
    ILogger<ComplianceNotificationJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = TimeUntilNextRun();
            logger.LogInformation("Compliance job sleeping for {Delay}", delay);
            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
                await RunAsync(stoppingToken);
        }
    }

    private async Task RunAsync(CancellationToken ct)
    {
        logger.LogInformation("Running daily compliance check");

        await using var scope       = services.CreateAsyncScope();
        var branchService           = scope.ServiceProvider.GetRequiredService<IBranchService>();
        var emailService            = scope.ServiceProvider.GetRequiredService<IEmailNotificationService>();
        var userManager             = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var db                      = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var threshold               = config.GetValue<double>("Notifications:ComplianceThresholdPct", 75.0);

        var now   = DateTime.UtcNow;
        var year  = now.Year;
        var month = now.Month;

        var compliance = await branchService.GetComplianceAsync(year, month);
        var below      = compliance.Branches.Where(b => b.FillRate < threshold).ToList();

        if (!below.Any()) return;

        var supervisors = await userManager.GetUsersInRoleAsync(AppRoles.FieldSupervisor);
        var planners    = await userManager.GetUsersInRoleAsync(AppRoles.PlannerDashboard);
        var recipients  = supervisors.Concat(planners).Where(u => u.IsActive && !string.IsNullOrEmpty(u.Email)).ToList();

        foreach (var branch in below)
        {
            foreach (var user in recipients)
            {
                await emailService.SendComplianceAlertAsync(
                    user.Email!, user.FullName, branch.BranchName, branch.FillRate, year, month);
            }
        }

        logger.LogInformation("Compliance check done. Notified {Count} branches below threshold", below.Count);
    }

    private TimeSpan TimeUntilNextRun()
    {
        var hourUtc = config.GetValue<int>("Notifications:DailyCheckHourUtc", 8);
        var now     = DateTime.UtcNow;
        var next    = new DateTime(now.Year, now.Month, now.Day, hourUtc, 0, 0, DateTimeKind.Utc);
        if (next <= now) next = next.AddDays(1);
        return next - now;
    }
}
