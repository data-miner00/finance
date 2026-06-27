namespace Provisioning;

internal interface IExecutor
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
