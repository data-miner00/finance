using System;
using System.Collections.Generic;
using System.Text;

namespace Provisioning.Activities
{
    internal interface IActivity
    {
        Task ProvisionAsync(CancellationToken cancellationToken);
    }
}
