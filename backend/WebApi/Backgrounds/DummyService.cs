namespace WebApi.Backgrounds;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class DummyService : BackgroundService
{
    private readonly ILogger<DummyService> logger;

    public DummyService(ILogger<DummyService> logger)
    {
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Background Task is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            this.logger.LogInformation($"Task executing at: {DateTimeOffset.Now}");

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }

        this.logger.LogInformation("Background Task is stopping.");
    }
}
