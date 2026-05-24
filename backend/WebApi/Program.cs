
using Core.Models;
using Core.Repositories;
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
            var builder = WebApplication.CreateBuilder(args);

            builder.ConfigureCors();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var connection = new SqlConnection(builder.Configuration.GetConnectionString("SQLServer"));

            builder.Services.AddSingleton<IDbConnection>(connection);
            builder.Services.AddSingleton<IRepository<Account>, AccountRepository>();
            builder.Services.AddSingleton<IRepository<Expense>, ExpenseRepository>();
            builder.Services.AddSingleton<IRepository<Income>, IncomeRepository>();
            builder.Services.AddSingleton<IRepository<PiggyBank>, PiggyBankRepository>();
            builder.Services.AddSingleton<IRepository<RecurringAction>, RecurringActionRepository>();

            var app = builder.Build();

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
