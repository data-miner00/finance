namespace Provisioning;

using Provisioning.Activities;
using System.Threading.Tasks;

internal sealed class Executor : IExecutor
{
    private readonly List<IActivity> activities;

    public Executor(List<IActivity> activities)
    {
        this.activities = activities;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // 1. Create Database
        // 2. Create Tables & SPs (manual)
        // 3. Seed Predefined Categories
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var activity in activities)
        {
            await activity.ProvisionAsync(cancellationToken);
        }
    }
}
