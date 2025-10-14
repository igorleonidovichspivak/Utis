
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Utis.Tasks.Domain.Interfaces.Repositories;
using Utis.Tasks.Domain.Interfaces.Services;
using Utis.Tasks.Infrastructure;
using Utis.Tasks.Infrastructure.Repositories;
using Utis.Tasks.WebApi.BackgroundServices;
using Utis.Tasks.WebApi.Services;

namespace Utis.Tasks.WebApi.Configuration
{
	public static class ApplicationConfig
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<StorageContext>(options =>
			{
				//if (EntityFrameworkLog) options.UseLoggerFactory(EFLoggerFactory);
				options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), 
				o => o.CommandTimeout(30)
					.SetPostgresVersion(13, 0))
				.UseSnakeCaseNamingConvention()
				.EnableSensitiveDataLogging()
				.EnableDetailedErrors();
			}, ServiceLifetime.Scoped, ServiceLifetime.Scoped);

			services.AddSingleton<Serilog.ILogger>(Log.Logger);

			services.AddSingleton<IRabbitMqService, RabbitMqService>();

			services.AddScoped<ITaskRepository, TaskRepository>();

			services.AddScoped<ITaskService, TaskService>();

			services.AddHostedService<OverdueService>();

			services.AddHttpClient<ICurrencyService, CurrencyService>()
				.AddPolicyHandler(GetRetryPolicy())
				.AddPolicyHandler(GetCircuitBreakerPolicy())
				.AddPolicyHandler(GetTimeoutPolicy());
			

			return services;
		}

		static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
		{
			return HttpPolicyExtensions
				.HandleTransientHttpError() // 5xx, 408 (Timeout), 502 (Bad Gateway), etc.
				.OrResult(msg => !msg.IsSuccessStatusCode)
				.WaitAndRetryAsync(
					retryCount: 3,
					sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
					onRetry: (outcome, timespan, retryCount) =>
					{
						var logger = Log.Logger; 
						logger?.Warning("Retry {RetryCount} after {Delay}ms for {Uri}. Status: {StatusCode}",
							retryCount, timespan.TotalMilliseconds,
							outcome.Result?.RequestMessage?.RequestUri,
							outcome.Result?.StatusCode);
					});
		}

		static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
		{
			return HttpPolicyExtensions
				.HandleTransientHttpError()
				.OrResult(msg => !msg.IsSuccessStatusCode)
				.CircuitBreakerAsync(
					handledEventsAllowedBeforeBreaking: 3,
					durationOfBreak: TimeSpan.FromSeconds(30),
					onBreak: (outcome, breakDelay) =>
					{
						var logger = Log.Logger;
						logger?.Error("Circuit breaker opened for {BreakDelay}ms. Uri: {Uri}, Status: {StatusCode}",
							breakDelay.TotalMilliseconds,
							outcome.Result?.RequestMessage?.RequestUri,
							outcome.Result?.StatusCode);
					},
					onReset: () =>
					{
						var logger = Log.Logger;
						logger?.Information("Circuit breaker reset");
					},
					onHalfOpen: () =>
					{
					});
		}

		static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
		{
			return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(15));
		}
	}
}
