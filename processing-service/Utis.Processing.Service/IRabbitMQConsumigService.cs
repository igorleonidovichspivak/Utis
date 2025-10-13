namespace Utis.Processing.Service
{
	public interface IRabbitMQConsumerService 
	{
		Task InitializeAsync();
		Task StartConsuming();
	}
}
