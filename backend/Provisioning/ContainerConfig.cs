namespace Provisioning;

using Autofac;
using Autofac.Configuration;
using Microsoft.Extensions.Configuration;
using Core.Models;
using Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Provisioning.Options;
using Provisioning.Activities;

internal static class ContainerConfig
{
    public static IContainer Configure()
    {
        var builder = new ContainerBuilder();

        builder
            .ConfigureOptions()
            .ConfigureDatabase()
            .ConfigureRepositories()
            .ConfigureActivities()
            .ConfigureEntryPoint();

        return builder.Build();
    }

    private static ContainerBuilder ConfigureOptions(this ContainerBuilder builder)
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonFile("settings.json");

        var config = configBuilder.Build();

        var module = new ConfigurationModule(configBuilder.Build());

        builder.RegisterModule(module);

        var databaseOption =
            config.GetSection(DatabaseOption.SectionName).Get<DatabaseOption>()
            ?? throw new InvalidOperationException("The database option is required.");

        builder.RegisterInstance(databaseOption);

        var expenseOption =
            config.GetSection(ExpenseOption.SectionName).Get<ExpenseOption>()
            ?? throw new InvalidOperationException("The expense option cannot be empty.");

        builder.RegisterInstance(expenseOption);

        return builder;
    }

    private static ContainerBuilder ConfigureDatabase(this ContainerBuilder builder)
    {
        builder.Register(
            ctx =>
            {
                var option = ctx.Resolve<DatabaseOption>();

                var connection = new SqlConnection(option.ConnectionString);

                return connection;
            })
            .As<IDbConnection>()
            .AsSelf()
            .SingleInstance();

        return builder;
    }

    private static ContainerBuilder ConfigureRepositories(this ContainerBuilder builder)
    {
        builder.RegisterType<CategoryRepository>().As<IRepository<Category>>().SingleInstance();

        return builder;
    }

    private static ContainerBuilder ConfigureEntryPoint(this ContainerBuilder builder)
    {
        builder.RegisterType<Executor>().As<IExecutor>().SingleInstance();

        return builder;
    }

    private static ContainerBuilder ConfigureActivities(this ContainerBuilder builder)
    {
        builder.RegisterType<ProvisionDatabaseActivity>().SingleInstance();
        builder.RegisterType<SeedCategoriesActivity>().SingleInstance();

        builder.Register(ctx =>
        {
            var activities = new List<IActivity>
            {
                ctx.Resolve<ProvisionDatabaseActivity>(),
                new VoidActivity("Manual step to provision tables."),
                ctx.Resolve<SeedCategoriesActivity>(),
            };

            return activities;
        })
            .As<List<IActivity>>();

        return builder;
    }
}
