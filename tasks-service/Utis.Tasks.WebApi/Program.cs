using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;
using System;
using System.Text.Json.Serialization;
using Utis.Tasks.Infrastructure;
using Utis.Tasks.WebApi.BackgroundServices;
using Utis.Tasks.WebApi.Configuration;
using Utis.Tasks.WebApi.Middlewares;


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
app.MapHealthChecks("/health");



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
