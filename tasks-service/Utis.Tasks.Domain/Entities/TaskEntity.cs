using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utis.Tasks.Domain.Entities
{
	public class TaskEntity
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime? DueDate { get; set; }
		public TaskState Status {  get; set; }

	}
}
