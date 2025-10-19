namespace Utis.Tasks.Domain.Interfaces.Services
{
	public interface IRabbitMqService
	{
		Task SendMessageAsync<T>(T message, CancellationToken cancellationToken = default);
		Task InitializeAsync(CancellationToken cancellationToken = default);
		Task PauseAsync(CancellationToken cancellationToken = default);
		Task ResumeAsync(CancellationToken cancellationToken = default);
	}
}
