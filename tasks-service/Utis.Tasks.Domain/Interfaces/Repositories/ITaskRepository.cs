using Utis.Tasks.Domain.Models;

namespace Utis.Tasks.Domain.Interfaces.Repositories
{
	public interface ITaskRepository
	{
		Task<int> Create(TaskModel newTask);
		Task<TaskModel> Get(int taskId);
		Task<bool> Update(TaskModel updatedTask);
		Task<bool> Delete(int taskId);
		Task<IEnumerable<TaskModel>> GetAll(TaskState? status);
		Task<(IEnumerable<TaskModel> Tasks, int TotalCount)> GetPagedFiltred(int page, int pageSize, TaskState? status);
		Task<IEnumerable<TaskModel>> GetOverdueTasks(DateTime onTime);
		Task<int> SetOverdueStatus(List<int> taskIds);
	}
}
