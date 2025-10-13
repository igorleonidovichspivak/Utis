
namespace Utis.Processing.Service
{
    public class MessageConsumer : BackgroundService
    {
        private readonly ILogger<MessageConsumer> _logger;
		private readonly IServiceProvider _serviceProvider;
		private IRabbitMQConsumerService _consumerService;

		public MessageConsumer(
			IServiceProvider serviceProvider,
			ILogger<MessageConsumer> logger)
        {
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
			await using var scope = _serviceProvider.CreateAsyncScope();
			_consumerService = scope.ServiceProvider.GetRequiredService<IRabbitMQConsumerService>();

			try
			{
				await _consumerService.InitializeAsync();
				await _consumerService.StartConsuming();
				_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

				while (!stoppingToken.IsCancellationRequested)
				{
					await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
				}
			}
			catch (OperationCanceledException)
			{
				// Expected during graceful shutdown
				_logger.LogInformation("RabbitMQ consumer operation cancelled");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Fatal error in RabbitMQ consumer");
				throw;
			}
        }

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Message Queue Background Service stopping...");

			if (_consumerService is IAsyncDisposable asyncDisposable)
			{
				await asyncDisposable.DisposeAsync();
			}
			else if (_consumerService is IDisposable disposable)
			{
				disposable.Dispose();
			}

			await base.StopAsync(cancellationToken);
		}
	}
}
