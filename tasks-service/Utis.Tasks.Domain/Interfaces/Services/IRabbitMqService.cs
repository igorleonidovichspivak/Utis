//using Microsoft.AspNetCore.Connections;
//using Microsoft.EntityFrameworkCore.Metadata;
//using Microsoft.EntityFrameworkCore.Metadata;
namespace Utis.Tasks.Domain.Interfaces.Services
{
	public interface IRabbitMqService
	{
		//void SendMessage(string queueName, string message);
		//void SendMessage<T>(string queueName, T message);
		//Task SendMessageAsync(string queueName, string message, CancellationToken cancellationToken = default);
		Task SendMessageAsync<T>(T message, CancellationToken cancellationToken = default);
		Task InitializeAsync();

	}


}
