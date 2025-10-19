using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utis.Tasks.WebApi.Configuration;
using Utis.Tasks.WebApi.Middlewares;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
	.ConfigureApiBehaviorOptions(options =>  
	{
		options.SuppressModelStateInvalidFilter = false; // Auto validation is on
	});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();


Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.WriteTo.Console()
	.Enrich.FromLogContext()
	.CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddMemoryCache();

builder.Services.Configure<RabbitMQOptions>(
	builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddApplicationServices(builder.Configuration);

// convert all enums to strings
builder.Services.Configure<JsonOptions>(options =>
{
	options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});



var app = builder.Build();

//auto db migration
//using (var scope = app.Services.CreateScope())
//{
//	var dbContext = scope.ServiceProvider.GetRequiredService<StorageContext>();
//	dbContext.Database.Migrate();
//}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
	ResponseWriter = async (context, report) =>
	{
		var result = new
		{
			status = report.Status.ToString(),
			timestamp = DateTime.UtcNow
		};

		context.Response.ContentType = "application/json";
		await context.Response.WriteAsync(JsonSerializer.Serialize(result));
	}
});

app.MapControllers();

try
{
	app.Run();
}
catch (Exception e)
{
	Log.Error(e, e.Message);
}
finally
{
	Log.CloseAndFlush();
}
