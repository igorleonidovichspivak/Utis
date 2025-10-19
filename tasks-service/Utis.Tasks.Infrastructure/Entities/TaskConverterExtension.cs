using Utis.Tasks.Domain.Models;

namespace Utis.Tasks.Infrastructure.Entities
{
	public static class TaskConverterExtension
	{
		public static TaskEntity ToEntity(this TaskModel entity)
		{
			return new TaskEntity
			{
				Id = entity.Id,
				Title = entity.Title,
				Description = entity.Description,
				DueDate = entity.DueDate,
				Status = entity.Status,
			};
		}

	}
}
