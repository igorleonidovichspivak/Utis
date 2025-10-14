using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utis.Tasks.Infrastructure;
using Utis.Tasks.WebApi.BackgroundServices;
using Utis.Tasks.WebApi.Configuration;
using Utis.Tasks.WebApi.Middlewares;
using Utis.Tasks.WebApi.Services;


var builder = WebApplication.CreateBuilder(args);

//MapsterConfig.Configure();

//builder.Services.AddMapster();
// Add services to the container.
builder.Services.AddControllers()
	.ConfigureApiBehaviorOptions(options =>  
	{
		options.SuppressModelStateInvalidFilter = false; // Auto validation is on
	});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

//TODO: add tracing middleware with opentel
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

//// Health check endpoint
//app.MapGet("/health", () =>
//{
//	return Results.Ok(new
//	{
//		status = "Healthy",
//		timestamp = DateTime.UtcNow,
//		service = "Currency API"
//	});
//})
//.WithName("HealthCheck");

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
