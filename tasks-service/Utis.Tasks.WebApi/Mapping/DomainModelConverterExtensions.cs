using Utis.Tasks.Domain.Models;
using Utis.Tasks.WebApi.Dtos;

namespace Utis.Tasks.WebApi.Mapping
{
	public static class DomainModelConverterExtensions
	{
		public static TaskDto ToDto(this TaskModel model)
		{
			return new TaskDto
			{
				Id = model.Id,
				Status = model.Status.ToString(),
				Title = model.Title,
				Description = model.Description,
				DueDate = model.DueDate
			};
		}

		public static TaskExpiredMessage ToExpiredMessage(this TaskModel model, DateTime detectAt)
		{
			return new TaskExpiredMessage
			{
				Id = model.Id,
				Title = model.Title,
				Description = model.Description,
				DueDate = model.DueDate,
				Status = model.Status,
				DetectAt = detectAt
			};
		}
	}
}