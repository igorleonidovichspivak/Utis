using System.ComponentModel.DataAnnotations;
using Utis.Tasks.Domain.Entities;

namespace Utis.Tasks.WebApi.Models
{
	public class PagedFiltredTasksRequest
	{
		[EnumDataType(typeof(TaskState), ErrorMessage = "Invalid task status")]
		public string? Status { get; set; }
		public int? Page { get; set; } = 1;
		public int? PageSize { get; set; } = 10;

	}
}
