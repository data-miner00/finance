using Core.Models;
using Core.Repositories;
using Provisioning.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Provisioning.Activities
{
    internal class SeedCategoriesActivity : IActivity
    {
        private readonly IRepository<Category> repository;
        private readonly ExpenseOption option;

        public SeedCategoriesActivity(IRepository<Category> repository, ExpenseOption option)
        {
            this.repository = repository;
            this.option = option;
        }

        public async Task ProvisionAsync(CancellationToken cancellationToken)
        {
            foreach (var category in this.option.DefaultCategories)
            {
                await this.repository.CreateAsync(new Category { Name = category }, cancellationToken);
                Console.WriteLine($"Provisioned category \"{category}\".");
            }

            Console.WriteLine("Done provisioning default categories.");
        }
    }
}
