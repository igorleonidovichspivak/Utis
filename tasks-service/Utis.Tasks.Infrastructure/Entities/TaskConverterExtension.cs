using Utis.Tasks.Domain.Models;

namespace Utis.Tasks.Infrastructure.Entities
{
	public static class TaskConverterExtension
	{
		public static TaskModel ToModel(this TaskEntity entity)
		{
			return new TaskModel
			{
				Id = entity.Id,
				Title = entity.Title,
				Description = entity.Description,
				DueDate = entity.DueDate,
				Status = (Domain.Models.TaskState)entity.Status,
			};
		}

		public static TaskEntity ToEntity(this TaskModel entity)
		{
			return new TaskEntity
			{
				Id = entity.Id,
				Title = entity.Title,
				Description = entity.Description,
				DueDate = entity.DueDate,
				Status = (TaskState)entity.Status,
			};
		}

	}
}
