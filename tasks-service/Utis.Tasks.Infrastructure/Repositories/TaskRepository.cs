using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utis.Tasks.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Utis.Tasks.Infrastructure.Repositories
{
	public class TaskRepository : ITaskRepository, IRepository
	{
		private readonly StorageContext Context;
		public TaskRepository(StorageContext context)
		{
			Context = context;	
		}

		public async Task<int> Create(TaskEntity newTask)
		{
			var createdTask = await Context.Tasks.AddAsync(newTask);

			await Context.SaveChangesAsync().ConfigureAwait(false);

			return createdTask.Entity.Id;
		}

		public async Task<TaskEntity> Get(int taskId)
		{
			var possibleTask = await Context.Tasks.FirstOrDefaultAsync(s => s.Id == taskId).ConfigureAwait(false);
			if (possibleTask == null)
			{
				throw new Exception($"Task with id {taskId} does not exist");
			}

			return possibleTask;
		}

		public async Task<bool> Update(TaskEntity updatedTask)
		{
			var existedTask = await Context.Tasks.AsTracking().FirstOrDefaultAsync(s => s.Id == updatedTask.Id);
			if (existedTask == null)
			{
				return false;
			}


			Context.Update(updatedTask);

			return await Context.SaveChangesAsync().ConfigureAwait(false) > 0;
		}

		public async Task<bool> Delete(int taskId)
		{
			var deletingTask = await Context.Tasks.FindAsync(taskId);
			if (deletingTask == null)
			{
				return false;
			}

			Context.Tasks.Remove(deletingTask);

			var affectedRows = await Context.SaveChangesAsync().ConfigureAwait(false);
			return affectedRows > 0;
		}

		

		public async Task<IEnumerable<TaskEntity>> GetAll()
		{
			return await Context.Tasks.ToListAsync().ConfigureAwait(false);
		}

		public async Task<(IEnumerable<TaskEntity> Tasks, int TotalCount)> GetPagedFiltred(int page, int pageSize, TaskState? status)
		{
			var query = Context.Tasks.AsQueryable();

			if (status.HasValue)
			{
				query = query.Where(t => t.Status == status.Value);
			}

			var totalCount = query.CountAsync();
			var tasks = query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();


			await Task.WhenAll(totalCount, tasks).ConfigureAwait(false);

			return (Tasks: tasks.Result, TotalCount: totalCount.Result);
		}
	}
}
