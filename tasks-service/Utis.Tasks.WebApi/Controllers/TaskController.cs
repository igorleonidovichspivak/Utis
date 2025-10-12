using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utis.Tasks.Domain.Entities;
using Utis.Tasks.Domain.Interfaces;
using Utis.Tasks.WebApi.Dtos;
using Utis.Tasks.WebApi.Models;
using Utis.Tasks.WebApi.Services;

namespace Utis.Tasks.WebApi.Controllers
{
	[ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        //todo: mapping should used done by Mapster or Automapper

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
		public async Task<IActionResult> Create([FromBody] NewTaskRequest request)
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


			var newTask = new TaskEntity
            {
                Status = TaskState.New,
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate
            };
			var taskId = await _taskService.Create(newTask);

			_logger.LogInformation($"Created successfully");
			
            return CreatedAtAction(nameof(Create), taskId);
		}

		// GET: api/tasks/5
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<TaskEntity>> Get(int id)
        {
			var task = await _taskService.Get(id);
			if (task == null)
				return NotFound($"Task with id {id} not found");

			//var taskDto = task.Adapt<TaskEntityDto>();
			return Ok(task);
		}

		// PUT: api/tasks/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> Update([FromBody] UpdateTaskRequest request)
        {
			_logger.LogInformation($"Updating task with id {request.Id}");

			_ = Enum.TryParse(request.Status, out TaskState state) ? state : state = TaskState.InProgress;
			var updatedTask = new TaskEntity
			{
				Status = state,
				Title = request.Title,
				Description = request.Description,
				DueDate = request.DueDate
			};
            var result = await _taskService.Update(updatedTask);

			_logger.LogInformation($"Update successfully");
			return result ? Ok($"Task with id {request.Id} updated successfully") : NotFound($"Task with id {request.Id} not found");
		}

		// DELETE: api/tasks/5
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> Delete(int id)
		{

			var result = await _taskService.Delete(id);
			

			return result ? NoContent() : NotFound($"Task with id {id} not found");

		}


		// GET: api/tasks?page=1&pageSize=10&status=InProgress
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<PagedFiltredTasksResponse>> GetAll([FromQuery] PagedFiltredTasksRequest? request)
		{
			IEnumerable<TaskEntity> result;

			//todo: handle bad requests
			//if request is valid 

			_ = Enum.TryParse(request!.Status, out TaskState state);

			var (tasks, totalCount) = await _taskService.GetPagedFiltred(request.Page!.Value, request.PageSize!.Value, state);

			var response = new //PagedResponse<TaskResponseDto>
			{
				Items = tasks,
				Page = request.Page,
				PageSize = request.PageSize,
				TotalCount = totalCount
			};

			return Ok(response);

		}
    }

    
}
