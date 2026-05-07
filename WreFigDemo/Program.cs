using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WreFigDemo.Components;
using WreFigDemo.Data;
using WreFigDemo.Data.Seed;
using WreFigDemo.Identity;
using WreFigDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// EF Core + Identity
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Factory for Blazor Server services — each operation gets its own short-lived context
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Scoped);

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit           = true;
        options.Password.RequireUppercase        = true;
        options.Password.RequiredLength          = 8;
        options.Password.RequireNonAlphanumeric  = true;
        options.SignIn.RequireConfirmedAccount    = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<AppUserClaimsPrincipalFactory>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath        = "/Account/Login";
    options.LogoutPath       = "/Account/Logout";
    options.AccessDeniedPath = "/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan   = TimeSpan.FromHours(8);
});

// Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Razor Pages (for Login/Logout)
builder.Services.AddRazorPages();

// App services
builder.Services.AddScoped<IAuditService,AuditService>();
builder.Services.AddScoped<IBranchService,BranchService>();
builder.Services.AddScoped<IScheduleService,ScheduleService>();
builder.Services.AddScoped<IStatusCodeService,StatusCodeService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IEmailNotificationService,EmailNotificationService>();
builder.Services.AddScoped<NoteViewStateService>();
builder.Services.AddScoped<AlertReadStateService>();
builder.Services.AddScoped<IMonthStateService, MonthStateService>();
builder.Services.AddHostedService<ComplianceNotificationJob>();

builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Run migrations and seed — wrapped so a DB config error shows a helpful log, not a silent crash
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await DataSeeder.SeedAsync(scope.ServiceProvider);
        logger.LogInformation("Database migration and seed completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Database startup failed. Check ConnectionStrings:DefaultConnection. Message: {Message}", ex.Message);
        // Re-throw so the app service restarts and the error appears in Azure logs
        throw;
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
