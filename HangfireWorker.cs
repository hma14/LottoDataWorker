using Hangfire;
using LottoDataWorker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class HangfireWorker : BackgroundService
{
    private readonly ILogger<HangfireWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public HangfireWorker(ILogger<HangfireWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Hangfire Job Scheduler...");

        using (var scope = _serviceProvider.CreateScope())
        {
            var seleniumJob = scope.ServiceProvider.GetRequiredService<SeleniumJob>();

            // 🔥 Run the Selenium scraper immediately on startup
            await seleniumJob.RunSeleniumScraper();
        }

        // Schedule job to run every day at midnight
        RecurringJob.AddOrUpdate<SeleniumJob>(
            "selenium-task",
            job => job.RunSeleniumScraper(),
            Cron.Daily
        );

    }
}
