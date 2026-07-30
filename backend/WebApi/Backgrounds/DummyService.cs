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

    public DummyService(ILogger<DummyService> logger, IServiceMetadataRepository metadataRepository)
    {
        this.logger = logger;
        this.metadataRepository = metadataRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Background Task is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            this.logger.LogInformation($"Task executing at: {DateTimeOffset.Now}");

            await this.metadataRepository.UpsertAsync(new ServiceMetadata
            {
                ServiceName = ServiceName,
            }, stoppingToken);

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }

        this.logger.LogInformation("Background Task is stopping.");
    }
}
