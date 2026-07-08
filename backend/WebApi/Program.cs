
using Core.Models;
using Core.Repositories;
using Core.Streams;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApi.Options;

namespace WebApi
{
    public static class Program
    {
        private static readonly string CorsPolicyName = "FinanceCorsPolicy";

        public static void Main(string[] args)
        {
            var app = WebApplication.CreateBuilder(args)
                .ConfigureServices()
                .Build();

            app.MapOpenApi();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/openapi/v1.json", "Finance API V1"); // /swagger/index
            });
            app.UseHttpsRedirection();
            app.UseCors(CorsPolicyName);
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        private static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.ConfigureCors();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var connection = new SqlConnection(builder.Configuration.GetConnectionString("SQLServer"));

            builder.Services.AddSingleton<IDbConnection>(connection);
            builder.Services.AddSingleton<IRepository<Account>, AccountRepository>();
            builder.Services.AddSingleton<ExpenseRepository>();
            builder.Services.AddSingleton<IRepository<Income>, IncomeRepository>();
            builder.Services.AddSingleton<IRepository<PiggyBank>, PiggyBankRepository>();
            builder.Services.AddSingleton<IRepository<RecurringAction>, RecurringActionRepository>();
            builder.Services.AddSingleton<IRepository<Category>, CategoryRepository>();
            builder.Services.AddSingleton<IRepository<Tax>, TaxRepository>();
            builder.Services.AddSingleton<IDictionary<string, IDataStreamifier>>((ctx) =>
            {
                return new Dictionary<string, IDataStreamifier>
                {
                    { "json", new JsonStreamifier() },
                    { "xml", new XmlStreamifier() },
                    { "csv", new CsvStreamifier() },
                };
            });

            return builder;
        }

        private static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
        {
            var corsOptions = builder.Configuration.GetSection("Cors").Get<CorsOptions>()
                ?? throw new InvalidOperationException("Cors option not found.");

            builder.Services.AddCors((opt) =>
            {
                opt.AddPolicy(
                    name: CorsPolicyName,
                    (policy) =>
                    {
                        policy.WithOrigins(corsOptions.AllowedOrigins)
                              .WithHeaders(corsOptions.AllowedHeaders)
                              .WithMethods(corsOptions.AllowedMethods);
                    });
            });

            return builder;
        }
    }
}
