using Utis.Tasks.Domain.Models;

namespace Utis.Tasks.WebApi.Models.Converters
{
	public static class TaskRequestExtensions
	{
		public static TaskModel ToModel(this NewTaskRequest request)
		{
			return new TaskModel
			{
				Status = TaskState.New,
				Title = request.Title,
				Description = request.Description,
				DueDate = request.DueDate
			};
		}

		public static TaskModel ToModel(this UpdateTaskRequest request)
		{
			_ = Enum.TryParse(request.Status, out TaskState state) ? state : state = TaskState.InProgress;
			return new TaskModel
			{
				Id = request.Id,
				Status = state,
				Title = request.Title,
				Description = request.Description,
				DueDate = request.DueDate
			};
		}

	}
}
