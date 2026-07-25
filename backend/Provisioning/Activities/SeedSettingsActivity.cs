using Core.Repositories;
using Provisioning.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Provisioning.Activities
{
    internal class SeedSettingsActivity : IActivity
    {
        private readonly ISettingsRepository repository;
        private readonly SettingsOption option;

        public SeedSettingsActivity(ISettingsRepository repository, SettingsOption option)
        {
            this.repository = repository;
            this.option = option;
        }

        public async Task ProvisionAsync(CancellationToken cancellationToken)
        {
            var existingKeys = (await this.repository.GetAllAsync(cancellationToken))
                .Select(s => s.Key)
                .ToHashSet();

            var missing = this.option.DefaultValues
                .Where(kv => !existingKeys.Contains(kv.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            if (missing.Count > 0)
            {
                await this.repository.UpsertManyAsync(missing, cancellationToken);
            }

            foreach (var key in this.option.DefaultValues.Keys)
            {
                Console.WriteLine($"Ensured default setting \"{key}\".");
            }

            Console.WriteLine("Done provisioning default settings.");
        }
    }
}
