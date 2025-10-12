using System.ComponentModel.DataAnnotations;
using Utis.Tasks.WebApi.Validation;

namespace Utis.Tasks.WebApi.Models
{
	public class NewTaskRequest
	{
		[Required(ErrorMessage = "Title is required")]
		[StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
		public string Title { get; set; } = string.Empty;

		[StringLength(250, ErrorMessage = "Description cannot be longer than 250 characters")]
		public string Description { get; set; } = string.Empty;

		[FutureDate(ErrorMessage = "Due date must be in the future")]
		public DateTime DueDate { get; set; }
		
	}
}
