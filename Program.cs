using Hangfire;
using Hangfire.SqlServer;
using LottoDataWorker;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using SeleniumLottoDataApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.AspNetCore.Hosting;


var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName,
    ContentRootPath = AppContext.BaseDirectory
});

// ✅ Add required services
builder.Services.AddScoped<SeleniumJob>(); // Ensure that your job is registered
builder.Services.AddLogging(); // Ensure logging is available


// Run as Windows Service
builder.Host.UseWindowsService(); // 👈 Important!

// Configure logging (set to Warning level to reduce logs)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Warning); // 👈 Reduce log output

// Connection string for SQL Server
#if true

    var baseConn = builder.Configuration.GetConnectionString("LottoDbContext");
    var password = Environment.GetEnvironmentVariable("LOTTO_DB_PASSWORD");

    if (string.IsNullOrWhiteSpace(password))
    {
        throw new InvalidOperationException("LOTTO_DB_PASSWORD is not set");
    }
    var connectionString = $"{baseConn};Password={password}";
#else

    var connectionString = builder.Configuration.GetConnectionString("LottoDbContext");

#endif


// Add DbContext
builder.Services.AddDbContext<LottoDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Hangfire
builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(connectionString, new SqlServerStorageOptions()));

builder.Services.AddHangfireServer();
builder.WebHost.UseUrls("http://localhost:5000");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

var app = builder.Build();

// Enable Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

//app.MapHangfireDashboard();

app.Lifetime.ApplicationStarted.Register(() =>
{
    RecurringJob.AddOrUpdate<SeleniumJob>(
        "selenium-job", // Unique job ID
        job => job.RunSeleniumScraper(),
                     //"0 10 * * *", // Cron schedule for 10:00 AM daily
        Cron.Daily,  // Equivalent to "0 0 * * *"
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Local } // Use local timezone
    );
});

app.Run();

