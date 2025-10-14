using Utis.Tasks.Domain.Entities;

namespace Utis.Tasks.Domain.Interfaces.Services
{
	public interface ITaskService
	{
		Task<int> Create(TaskEntity newTask);
		Task<TaskEntity> Get(int taskId);
		Task<bool> Update(TaskEntity newTask);
		Task<bool> Delete(int taskId);

		Task<IEnumerable<TaskEntity>> GetAll(TaskState? status = null);
		Task<(IEnumerable<TaskEntity> Tasks, int TotalCount)> GetPagedFiltred(int page, int pageSize, TaskState? status = null);
	}
}