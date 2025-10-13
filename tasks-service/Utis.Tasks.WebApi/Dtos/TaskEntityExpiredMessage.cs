using Utis.Tasks.Domain.Entities;

namespace Utis.Tasks.WebApi.Dtos
{
	public class TaskEntityExpiredMessage : TaskEntity
	{
		public DateTime? DetectAt { get; set; }
	}
}