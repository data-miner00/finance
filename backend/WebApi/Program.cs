
using Core.Repositories;
using WebApi.Options;

namespace WebApi
{
    public static class Program
    {
        private static readonly string CorsPolicyName = "FinanceCorsPolicy";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.ConfigureCors();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddSingleton(typeof(IRepository<>), typeof(MemoryRepository<>));

            var app = builder.Build();

            app.MapOpenApi();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/openapi/v1.json", "Finance API V1"); // /swagger/index
            });

            app.UseHttpsRedirection();

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
