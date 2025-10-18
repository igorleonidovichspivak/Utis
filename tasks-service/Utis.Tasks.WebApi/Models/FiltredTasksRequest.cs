using System.ComponentModel.DataAnnotations;
using Utis.Tasks.Domain.Models;

namespace Utis.Tasks.WebApi.Models
{
	public class FiltredTasksRequest
	{
		[EnumDataType(typeof(TaskState), ErrorMessage = "Invalid task status")]
		public string? Status { get; set; }
	}
}
