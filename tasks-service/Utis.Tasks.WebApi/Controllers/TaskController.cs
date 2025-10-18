using Mapster;
using Microsoft.AspNetCore.Mvc;
using Utis.Tasks.Domain.Interfaces.Services;
using Utis.Tasks.Domain.Models;

using Utis.Tasks.WebApi.Dtos;
using Utis.Tasks.WebApi.Mapping;
using Utis.Tasks.WebApi.Models;
using Utis.Tasks.WebApi.Models.Converters;


namespace Utis.Tasks.WebApi.Controllers
{
	[ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        //todo: mapping should done by Mapster or Automapper

        private readonly ILogger<TaskController> _logger;
		private readonly ITaskService _taskService;

		public TaskController(ILogger<TaskController> logger, ITaskService taskService)
        {
            _logger = logger;
            _taskService = taskService;
        }

		// POST: api/tasks
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<int>> Create([FromBody] NewTaskRequest request)
		{
            _logger.LogInformation($"Creating new task {request}");

			if (!ModelState.IsValid)
			{
				return BadRequest(new ValidationProblemDetails(ModelState)
				{
					Title = "Invalid request",
					Status = StatusCodes.Status400BadRequest
				});
			}


			var taskId = await _taskService.Create(request.ToModel());

			_logger.LogInformation($"Created successfully");
			
            return CreatedAtAction(nameof(Create), taskId);
		}

		// GET: api/tasks/5
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<TaskDto>> Get(int id)
        {
			var task = await _taskService.Get(id);
			if (task == null)
				return NotFound($"Task with id {id} not found");

			//var taskDto = task.Adapt<TaskEntityDto>();
			return Ok(task.ToDto());
		}

		// PUT: api/tasks/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<int>> Update([FromBody] UpdateTaskRequest request)
        {
			_logger.LogInformation($"Updating task with id {request.Id}");

			
            var result = await _taskService.Update(request.ToModel());

			_logger.LogInformation($"Update successfully");
			return result ? Ok($"Task with id {request.Id} updated successfully") : NotFound($"Task with id {request.Id} not found");
		}

		// DELETE: api/tasks/5
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<bool>> Delete(int id)
		{
			var result = await _taskService.Delete(id);
			
			return result ? NoContent() : NotFound($"Task with id {id} not found");
		}


		// GET: api/tasks?page=1&pageSize=10&status=InProgress
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<PagedResponse<TaskDto>>> GetAll([FromQuery] PagedFiltredTasksRequest? request)
		{
			//cannot filter by fake status, but null/empty status is allowed
			if (!string.IsNullOrEmpty(request!.Status) & !Enum.TryParse(request!.Status, out TaskState _))
			{
				return BadRequest($"Invalid status value: {request.Status}");
			}

			TaskState? status = null;
			if (!string.IsNullOrEmpty(request.Status) &&
				Enum.TryParse(request.Status, out TaskState parsedStatus))
			{
				status = parsedStatus;
			}

			// 1 case: non-paged results (all tasks or filtered by status)
			if (!request.Page.HasValue || !request.PageSize.HasValue)
			{
				var allTasks = await _taskService.GetAll(status);

				return Ok(new PagedResponse<TaskDto>
				{
					Items = [.. allTasks.Select(s => s.ToDto())],
					Page = 1,
					PageSize = allTasks.Count(),
					TotalCount = allTasks.Count()
				});
			}

			// 2 case: paged results (with optional status filter)
			var (tasks, totalCount) = await _taskService.GetPagedFiltred(request.Page!.Value, request.PageSize!.Value, status);

			return Ok(new PagedResponse<TaskDto>
			{
				Items = tasks.Select(s => s.ToDto()).ToList(),
				Page = request.Page.Value,
				PageSize = request.PageSize.Value,
				TotalCount = totalCount
			});
		}
		
	}

    
}
