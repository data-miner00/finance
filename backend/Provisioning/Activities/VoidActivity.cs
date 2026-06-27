using System;
using System.Collections.Generic;
using System.Text;

namespace Provisioning.Activities
{
    internal class VoidActivity : IActivity
    {
        private readonly string reason;

        public VoidActivity(string reason)
        {
            this.reason = reason;
        }

        public async Task ProvisionAsync(CancellationToken cancellationToken)
        {
            await Console.Out.WriteLineAsync("Void step because " + this.reason);
            Console.ReadLine();
        }
    }
}
