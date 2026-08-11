namespace WebApi.Backgrounds;

using Core.Models;
using Core.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class DummyService : BackgroundService
{
    private const string ServiceName = nameof(DummyService);
    private readonly ILogger<DummyService> logger;
    private readonly IServiceMetadataRepository metadataRepository;
    private readonly TimeProvider timeProvider;

    public DummyService(
        ILogger<DummyService> logger,
        IServiceMetadataRepository metadataRepository,
        TimeProvider timeProvider)
    {
        this.logger = logger;
        this.metadataRepository = metadataRepository;
        this.timeProvider = timeProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("{ServiceName} is starting.", ServiceName);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = this.timeProvider.GetUtcNow();
            this.logger.LogInformation("Task executing at: {Time}", now);

            await this.metadataRepository.UpsertAsync(new ServiceMetadata
            {
                ServiceName = ServiceName,
            }, stoppingToken);

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }

        this.logger.LogInformation("{ServiceName} is stopping.", ServiceName);
    }
}
