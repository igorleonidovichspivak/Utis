using System.Security.Authentication;
using System.Text.Json;
using ILogger = Serilog.ILogger;

namespace Utis.Tasks.WebApi.Middlewares
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;

		public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				var msg = $"Unhandled exception occurred while processing the request. {ex.Message} {ex.StackTrace}";
				_logger.Error(msg);


				var statusCode = ex switch
				{
					ArgumentException => StatusCodes.Status400BadRequest,
					_ => StatusCodes.Status500InternalServerError,
				};

				context.Response.StatusCode = statusCode;


				context.Response.ContentType = "application/json";

				var response = new
				{
					error = "An unexpected error occurred",
					detail = ex.Message 
				};

				var json = JsonSerializer.Serialize(response);
				await context.Response.WriteAsync(json);
			}
		}
	}
}
