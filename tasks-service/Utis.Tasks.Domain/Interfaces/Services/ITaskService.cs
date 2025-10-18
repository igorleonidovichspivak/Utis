using Utis.Tasks.Domain.Models;

namespace Utis.Tasks.Domain.Interfaces.Services
{
	public interface ITaskService
	{
		Task<int> Create(TaskModel newTask);
		Task<TaskModel> Get(int taskId);
		Task<bool> Update(TaskModel newTask);
		Task<bool> Delete(int taskId);

		Task<IEnumerable<TaskModel>> GetAll(TaskState? status = null);
		Task<(IEnumerable<TaskModel> Tasks, int TotalCount)> GetPagedFiltred(int page, int pageSize, TaskState? status = null);
	}
}