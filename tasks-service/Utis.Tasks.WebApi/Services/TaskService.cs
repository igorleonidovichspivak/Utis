using Utis.Tasks.Domain.Interfaces.Repositories;
using Utis.Tasks.Domain.Interfaces.Services;
using Utis.Tasks.Domain.Models;


namespace Utis.Tasks.WebApi.Services
{
	public class TaskService: ITaskService
	{
		readonly ILogger _logger;
		readonly ITaskRepository _taskRepository;
		public TaskService(ILogger<TaskService> logger, ITaskRepository taskRepository) 
		{
			_logger = logger;
			_taskRepository = taskRepository;
		}

		public async Task<int> Create(TaskModel newTask)
		{
			var result = await _taskRepository.Create(newTask);
			_logger.LogInformation($"Task with title {newTask.Title} and description {newTask.Description} and over due date {newTask.DueDate} created with such id {result}");

			return result;
		}

		public async Task<TaskModel> Get(int taskId)
		{
			var result =  await _taskRepository.Get(taskId);
			_logger.LogInformation($"Successfully get task by id {taskId}");

			return result;
		}

		public async Task<bool> Update(TaskModel updatedTask)
		{
			var succefullUpdate = await _taskRepository.Update(updatedTask);

			if (succefullUpdate)
			{
				_logger.LogInformation($"Successfully update task with id {updatedTask.Id} with such field: title - {updatedTask.Title}; description - {updatedTask.Description}, due date - {updatedTask.DueDate}");
			}
			else
			{
				_logger.LogInformation($"Cannot update task with id {updatedTask.Id}");
			}

			return succefullUpdate;
		}

		public async Task<bool> Delete(int taskId)
		{
			var deleteSucessfull = await _taskRepository.Delete(taskId);

			if (deleteSucessfull)
			{
				_logger.LogInformation($"Delete task with id {taskId} was succesfull");
			}
			else
			{
				_logger.LogInformation($"Delete task with id {taskId} was not succesfull");
			}
				

			return deleteSucessfull;
		}

		public async Task<IEnumerable<TaskModel>> GetAll(TaskState? status)
		{
			return await _taskRepository.GetAll(status);
		}
		public async Task<(IEnumerable<TaskModel> Tasks, int TotalCount)> GetPagedFiltred(int page, int pageSize, TaskState? status)
		{
			return await _taskRepository.GetPagedFiltred(page, pageSize, status);
		}

		

	}
}
