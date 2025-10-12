namespace Utis.Tasks.WebApi.Models
{

	public class PagedFiltredTasksRequest : FiltredTasksRequest
	{
		public int? Page { get; set; }
		public int? PageSize { get; set; }

	}
}
