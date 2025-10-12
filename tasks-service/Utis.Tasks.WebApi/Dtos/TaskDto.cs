using Utis.Tasks.Domain.Entities;

namespace Utis.Tasks.WebApi.Dtos
{
	public class TaskEntityDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime? DueDate { get; set; }
		public string Status { get; set; } = TaskState.New.ToString();
	}
}