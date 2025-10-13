using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Utis.Tasks.Domain.Entities;
using Utis.Tasks.Domain.Interfaces.Repositories;
using Utis.Tasks.Domain.Interfaces.Services;
using Utis.Tasks.WebApi.Dtos;
using Utis.Tasks.WebApi.Services;

namespace Utis.Tasks.WebApi.BackgroundServices
{
	public class OverdueService: BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<OverdueService> _logger;
		private readonly PeriodicTimer _timer;
		//private readonly int _batchSize = 100;

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

			while (await _timer.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
			{
				try
				{
					using (var scope = _serviceProvider.CreateScope())
					{
						var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
						var rabbitMqService = scope.ServiceProvider.GetRequiredService<IRabbitMqService>();
						await rabbitMqService.InitializeAsync();

						
						await CheckOverdueTasksAsync(taskRepository, rabbitMqService, cancellationToken);
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error checking overdue tasks");
				}
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
					foreach (var overdueTask in overdueTasks)
					{

						var message = new TaskEntityExpiredMessage
						{
							Id = overdueTask.Id,
							Title = overdueTask.Title,
							Description = overdueTask.Description,
							DueDate = overdueTask.DueDate,
							Status = overdueTask.Status,
							DetectAt = currentTime
						};

						await rabbitMq.SendMessageAsync(message, cancellationToken);

					}
					// может быть rabbitMq.SendBatchedMessage(overdueTasks)

					// Помечаем задачи как просроченные
					var taskIds = overdueTasks.Select(t => t.Id).ToList();

					_logger.LogInformation($"Got {taskIds.Count} tasks overdue on time {currentTime}");

					// Отправляем уведомления
					//await notificationService.SendBatchOverdueNotificationAsync(overdueTasks);

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
				throw;
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
