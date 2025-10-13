using Utis.Tasks.Domain.Entities;

namespace Utis.Tasks.Domain.Interfaces.Repositories
{
	public interface ITaskRepository
	{
		Task<int> Create(TaskEntity newTask);
		Task<TaskEntity> Get(int taskId);
		Task<bool> Update(TaskEntity updatedTask);
		Task<bool> Delete(int taskId);
		Task<IEnumerable<TaskEntity>> GetAll(TaskState? status);
		Task<(IEnumerable<TaskEntity> Tasks, int TotalCount)> GetPagedFiltred(int page, int pageSize, TaskState? status);
		Task<IEnumerable<TaskEntity>> GetOverdueTasks(DateTime onTime);
		Task<int> SetOverdueStatus(List<int> taskIds);
	}
}
