using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Utis.Processing.Service
{
	public class RabbitMQConsumerService : IRabbitMQConsumerService, IDisposable, IAsyncDisposable
	{
		private readonly ConnectionFactory _connectionFactory;
		private readonly string _queueName;
		private IConnection _connection;
		private IChannel _channel;
		private readonly ILogger<RabbitMQConsumerService> _logger;

		private bool _initialized = false;
		private bool _disposed = false;

		public RabbitMQConsumerService(IConfiguration configuration, ILogger<RabbitMQConsumerService> logger)
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
		}

		public async Task InitializeAsync()
		{
			try
			{
				_connection = await _connectionFactory.CreateConnectionAsync();
				_channel = await _connection.CreateChannelAsync();


				// Объявляем очередь
				await _channel.QueueDeclareAsync(
					queue: _queueName,
					durable: true,
					exclusive: false,
					autoDelete: false,
					arguments: null);

				// Ограничиваем количество неподтвержденных сообщений
				await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);


				_initialized = true;
				_logger.LogInformation("RabbitMQ connection established");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to connect to RabbitMQ");
				throw;
			}

		}

		public void Dispose()
		{
			DisposeAsync().AsTask().GetAwaiter().GetResult();
		}

		public async Task StartConsuming()
		{
			if (!_initialized)
			{
				await InitializeAsync();
			}
				
			try
			{
				var consumer = new AsyncEventingBasicConsumer(_channel);
				consumer.ReceivedAsync += async (model, ea) =>
				{
					try
					{
						var message = new
						{
							Id = ea.BasicProperties.MessageId ?? Guid.NewGuid().ToString(),
							Body = Encoding.UTF8.GetString(ea.Body.ToArray())
						};

						_logger.LogInformation("Received message: {MessageId} with body {Body}", message.Id, message.Body);

					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error processing message");
					}

				};

				await _channel.BasicConsumeAsync(_queueName, autoAck: true, consumer: consumer);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed consuming messages");
			}

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
