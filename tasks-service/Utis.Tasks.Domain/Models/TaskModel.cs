namespace Utis.Tasks.Domain.Models
{

	public class TaskModel
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime? DueDate { get; set; }
		public TaskState Status {  get; set; }

	}
}
