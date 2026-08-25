
using System.Threading.RateLimiting;
using Azure.Storage.Blobs;
using Core;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.Storage;
using Core.Streams;
using WebApi.Backgrounds;
using WebApi.Options;

namespace WebApi
{
    public static class Program
    {
        private static readonly string CorsPolicyName = "FinanceCorsPolicy";

        public static async Task Main(string[] args)
        {
            var app = WebApplication.CreateBuilder(args)
                .ConfigureServices()
                .Build();

            var blobServiceClient = app.Services.GetRequiredService<BlobServiceClient>();
            await blobServiceClient.GetBlobContainerClient(BlobContainerNames.Receipts).CreateIfNotExistsAsync();

            app.MapOpenApi();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/openapi/v1.json", "Finance API V1"); // /swagger/index
            });
            app.UseHttpsRedirection();
            app.UseCors(CorsPolicyName);
            app.UseRateLimiter();
            app.UseAuthorization();
            app.MapControllers();
            await app.RunAsync();
        }

        private static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.ConfigureCors();
            builder.ConfigureRateLimiting();
            builder.ConfigureOptions();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var connectionString = builder.Configuration.GetConnectionString("SQLServer")
                ?? throw new InvalidOperationException("SQL Server connection string not found.");

            builder.Services.AddSingleton<IDbConnectionFactory>(new SqlConnectionFactory(connectionString));

            var blobStorageConnectionString = builder.Configuration.GetConnectionString("AzureBlobStorage")
                ?? throw new InvalidOperationException("Azure Blob Storage connection string not found.");

            builder.Services.AddSingleton(new BlobServiceClient(blobStorageConnectionString));
            builder.Services.AddSingleton<IReceiptStorage, BlobReceiptStorage>();
            builder.Services.AddSingleton<IRepository<Account>, AccountRepository>();
            builder.Services.AddSingleton<ExpenseRepository>();
            builder.Services.AddSingleton<IRepository<Expense>>(sp => sp.GetRequiredService<ExpenseRepository>());
            builder.Services.AddSingleton<IRepository<Income>, IncomeRepository>();
            builder.Services.AddSingleton<IRepository<PiggyBank>, PiggyBankRepository>();
            builder.Services.AddSingleton<IRepository<RecurringAction>, RecurringActionRepository>();
            builder.Services.AddSingleton<CategoryRepository>();
            builder.Services.AddSingleton<IRepository<Category>>(sp => sp.GetRequiredService<CategoryRepository>());
            builder.Services.AddSingleton<IRepository<Tax>, TaxRepository>();
            builder.Services.AddSingleton<IProfileRepository, ProfileRepository>();
            builder.Services.AddSingleton<ISettingsRepository, SettingsRepository>();
            builder.Services.AddSingleton<IServiceMetadataRepository, ServiceMetadataRepository>();
            builder.Services.AddSingleton<INotificationRepository, NotificationRepository>();
            builder.Services.AddSingleton<IBudgetAlertService, BudgetAlertService>();
            builder.Services.AddSingleton<IDictionary<string, IDataStreamifier>>((ctx) =>
            {
                return new Dictionary<string, IDataStreamifier>
                {
                    { "json", new JsonStreamifier() },
                    { "xml", new XmlStreamifier() },
                    { "csv", new CsvStreamifier() },
                };
            });
            builder.Services.AddSingleton(TimeProvider.System);

            builder.Services.AddHostedService<RecurrentActionService>();

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

        private static WebApplicationBuilder ConfigureRateLimiting(this WebApplicationBuilder builder)
        {
            var rateLimitingOptions = builder.Configuration.GetSection(RateLimitingOptions.SectionName).Get<RateLimitingOptions>()
                ?? throw new InvalidOperationException("Rate limiting option not found.");

            builder.Services.AddRateLimiter(opt =>
            {
                opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                opt.OnRejected = async (context, cancellationToken) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();
                    }

                    await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
                };

                opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimitingOptions.PermitLimit,
                        Window = TimeSpan.FromSeconds(rateLimitingOptions.WindowSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = rateLimitingOptions.QueueLimit,
                    });
                });
            });

            return builder;
        }

        private static WebApplicationBuilder ConfigureOptions(this WebApplicationBuilder builder)
        {
            var recurrentOptions = builder.Configuration.GetSection(RecurrentActionServiceOptions.SectionName).Get<RecurrentActionServiceOptions>()
                ?? throw new InvalidOperationException("Recurrent action service option not found.");

            builder.Services.AddSingleton(recurrentOptions);

            return builder;
        }
    }
}
