using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Utis.Tasks.Domain.Interfaces.Services;
using Utis.Tasks.WebApi.Configuration;

namespace Utis.Tasks.WebApi.Services
{

	public class RabbitMqService : IRabbitMqService, IDisposable, IAsyncDisposable
	{
		private readonly ConnectionFactory _connectionFactory;
		private readonly string _queueName;
		private IConnection _connection;
		private IChannel _channel;
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
		private readonly ILogger<RabbitMqService> _logger;
		private readonly JsonSerializerOptions _jsonSerializationOptions;

		private bool _initialized = false;
		private bool _disposed = false;


		public RabbitMqService(ILogger<RabbitMqService> logger, IOptions<RabbitMQOptions> settings)
		{
			_logger = logger;
			_connectionFactory = new ConnectionFactory()
			{
				HostName = settings.Value.HostName,
				UserName = settings.Value.UserName,
				Password = settings.Value.Password,
				Port = settings.Value.Port,
				VirtualHost = settings.Value.VirtualHost
			};

			_queueName = settings.Value.QueueName;

			_jsonSerializationOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};
		}

		public async Task InitializeAsync(CancellationToken cancellationToken = default)
		{
			if (_initialized || _disposed)
				return;

			await _semaphore.WaitAsync(cancellationToken);
			try
			{
				//double checking, if yet initialized after locking 
				if (_initialized || _disposed)
					return;

				_connection = await _connectionFactory.CreateConnectionAsync();
				_channel = await _connection.CreateChannelAsync();

				_initialized = true;
				_logger.LogInformation("RabbitMQ connection established");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to connect to RabbitMQ");
				throw;
			}
			finally
			{
				_semaphore.Release();
			}
		}

		private async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
		{
			

			await _semaphore.WaitAsync(cancellationToken);
			try
			{
				// Объявляем очередь (создаем если не существует)
				await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);

				var body = Encoding.UTF8.GetBytes(message);
				var props = new BasicProperties
				{
					ContentType = "text/plain",
					DeliveryMode = DeliveryModes.Persistent
				};

				
				await _channel.BasicPublishAsync(exchange: "",
										routingKey: _queueName,
										mandatory: true,
										basicProperties: props,
										body: body,
										cancellationToken: cancellationToken);
				

				_logger.LogInformation($"Message {message} sent to queue {_queueName}");
			}
			catch (OperationCanceledException)
			{
				_logger.LogWarning($"Message sending was cancelled for queue {_queueName}");
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to send message to RabbitMQ queue {_queueName}");
				throw;
			}
			finally
			{
				_semaphore.Release();
			}

		}

		public async Task SendMessageAsync<T>(T message, CancellationToken cancellationToken = default)
		{
			if (!_initialized)
				return;

			var jsonMessage = JsonSerializer.Serialize(message, _jsonSerializationOptions);

			await SendMessageAsync(jsonMessage, cancellationToken);
		}


		public async Task PauseAsync(CancellationToken cancellationToken = default)
		{
			if (_channel?.IsOpen == true)
			{
				await _channel.CloseAsync(cancellationToken: cancellationToken);
				_logger.LogInformation("RabbitMQ channel paused for queue: {QueueName}", _queueName);
			}
		}

		public async Task ResumeAsync(CancellationToken cancellationToken = default)
		{
			if (_channel != null && !_channel.IsOpen && _connection?.IsOpen == true)
			{
				_channel = await _connection.CreateChannelAsync();
				_logger.LogInformation("RabbitMQ channel resumed for queue: {QueueName}", _queueName);
			}
		}
		public void Dispose()
		{
			DisposeAsync().AsTask().GetAwaiter().GetResult();
		}

		public async ValueTask DisposeAsync()
		{
			if (_disposed) return;

			_disposed = true;

			await _semaphore.WaitAsync();
			try
			{
				if (_channel?.IsOpen == true)
					await _channel.CloseAsync();

				if (_connection?.IsOpen == true)
					await _connection.CloseAsync();

				_channel?.DisposeAsync();
				_connection?.DisposeAsync();
			}
			finally
			{
				_semaphore.Release();
				_semaphore.Dispose();
			}
		}
	}


}
