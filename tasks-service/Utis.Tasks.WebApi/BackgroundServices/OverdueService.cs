using Utis.Tasks.Domain.Interfaces.Repositories;
using Utis.Tasks.Domain.Interfaces.Services;
using Utis.Tasks.Domain.Models;
using Utis.Tasks.WebApi.Mapping;

namespace Utis.Tasks.WebApi.BackgroundServices
{
	public class OverdueService: BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<OverdueService> _logger;
		private readonly PeriodicTimer _timer;

		private const int CheckingPeriodInMinutes = 1;
		public OverdueService(IServiceProvider serviceProvider, ILogger<OverdueService> logger) 
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
			_timer = new PeriodicTimer(TimeSpan.FromMinutes(CheckingPeriodInMinutes));
		}
		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Overdue Tasks Background Service started");

			using var scope = _serviceProvider.CreateScope();
			var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
			var rabbitMqService = scope.ServiceProvider.GetRequiredService<IRabbitMqService>();

			while (await _timer.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
			{

				await CheckOverdueTasksAsync(taskRepository, rabbitMqService, cancellationToken);
			}

			_logger.LogInformation("Overdue Tasks Background Service stopped");
		}
		private async Task CheckOverdueTasksAsync(ITaskRepository taskRepository, IRabbitMqService rabbitMq, CancellationToken cancellationToken)
		{
			var currentTime = DateTime.UtcNow;
			
			try
			{
				// Получаем задачи, которые стали просроченными
				var overdueTasks = await taskRepository.GetOverdueTasks(currentTime);

				if (overdueTasks.Any())
				{
					await SendMessageToRabbit(rabbitMq, currentTime, overdueTasks, cancellationToken);

					// Помечаем задачи как просроченные
					var taskIds = overdueTasks.Select(t => t.Id).ToList();
					_logger.LogInformation($"Got {taskIds.Count} tasks overdue on time {currentTime}");

					await taskRepository.SetOverdueStatus(taskIds);
				}
				else
				{
					_logger.LogInformation("Did not find overdue tasks");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in overdue tasks check");
			}
		}

		private async Task SendMessageToRabbit(IRabbitMqService rabbitMq, DateTime currentTime, IEnumerable<TaskModel> overdueTasks, CancellationToken cancellationToken)
		{
			try
			{
				await rabbitMq.InitializeAsync(cancellationToken);

				var batch = overdueTasks.Select(s => s.ToExpiredMessage(currentTime)).ToArray();

				await rabbitMq.SendMessageAsync(batch, cancellationToken);
				await rabbitMq.PauseAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to send expired tasks to RabbitMQ");
			}
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Overdue Tasks Background Service is stopping...");
			await base.StopAsync(cancellationToken);
		}

		public override void Dispose()
		{
			_timer?.Dispose();
			base.Dispose();
		}

	}
}
