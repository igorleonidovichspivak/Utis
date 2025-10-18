using Utis.Tasks.Domain.Models;

namespace Utis.Tasks.WebApi.Dtos
{
	public class TaskExpiredMessage : TaskModel
	{
		public DateTime? DetectAt { get; set; }
	}
}