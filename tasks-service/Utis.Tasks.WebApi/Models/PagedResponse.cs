﻿namespace Utis.Tasks.WebApi.Models
{
	public class PagedResponse<T>
	{
		public List<T> Items { get; set; } = new();
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }
	}
}