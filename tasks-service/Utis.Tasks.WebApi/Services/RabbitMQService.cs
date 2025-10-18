using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Utis.Tasks.Domain.Interfaces.Services;

namespace Utis.Tasks.WebApi.Services
{
	public class RabbitMQOptions
	{
		public string HostName { get; set; } = "localhost";
		public int Port { get; set; } = 5672;
		public string UserName { get; set; } = "guest";
		public string Password { get; set; } = "guest";
		public string VirtualHost { get; set; } = "/";
		public string ClientName { get; set; } = "RabbitMQClient";

		//
		public string QueueName{ get; set; } = "expired-tasks";

	}

	public class RabbitMqService : IRabbitMqService, IDisposable
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


		public RabbitMqService(IConfiguration configuration, ILogger<RabbitMqService> logger)
		{
			_logger = logger;
			_connectionFactory = new ConnectionFactory()
			{
				HostName = configuration["RabbitMQ:HostName"]!,
				UserName = configuration["RabbitMQ:UserName"]!,
				Password = configuration["RabbitMQ:Password"]!,
				Port = int.Parse(configuration["RabbitMQ:Port"]!),
				VirtualHost = configuration["RabbitMQ:VirtualHost"]!
			};

			_queueName = configuration["RabbitMQ:QueueName"]!;

			_jsonSerializationOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};
		}

		public async Task InitializeAsync()
		{
			try
			{
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
			
		}

		private async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
		{
			

			await _semaphore.WaitAsync(cancellationToken);
			try
			{
				// Объявляем очередь (создаем если не существует)
				await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken); //agruments: null

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

		public void Dispose()
		{
			_semaphore.Dispose();
			DisposeAsync().AsTask().GetAwaiter().GetResult();
		}

		public async ValueTask DisposeAsync()
		{
			if (_disposed) return;

			_disposed = true;

			if (_channel?.IsOpen == true)
				await _channel.CloseAsync();

			if (_connection?.IsOpen == true)
				await _connection.CloseAsync();

			
			_channel?.DisposeAsync();
			_connection?.DisposeAsync();
		}
	}


}
