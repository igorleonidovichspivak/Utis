using Mapster;
using Utis.Tasks.Domain.Entities;
using Utis.Tasks.WebApi.Dtos;
using Utis.Tasks.WebApi.Models;

namespace Utis.Tasks.WebApi.Mapping
{
	public class TaskMappingConfig : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			// Domain Task -> TaskDto
			config.NewConfig<TaskEntity, TaskEntityDto>()
				.Map(dest => dest.Status, src => src.Status.ToString());

			// CreateTaskRequest -> Domain Task
			config.NewConfig<NewTaskRequest, TaskEntity>()
				.Map(dest => dest.Status, _ => TaskState.New);

			// UpdateTaskRequest -> Domain Task
			config.NewConfig<UpdateTaskRequest, TaskEntity>()
				.Map(dest => dest.Status, src => Enum.Parse<TaskState>(src.Status));

			// TaskDto -> Domain Task (для обратного маппинга если нужно)
			config.NewConfig<TaskEntityDto, TaskEntity>()
				.Map(dest => dest.Status, src => Enum.Parse<TaskState>(src.Status));
		}
	}

}
