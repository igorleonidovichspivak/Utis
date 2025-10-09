using Utis.Tasks.WebApi.Configuration;
using Serilog;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//TODO: add tracing middleware with opentel
//app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();
app.MapHealthChecks("/health");
app.MapGet("/myhealth", () => "Service is running!");


app.MapControllers();

app.Run();
