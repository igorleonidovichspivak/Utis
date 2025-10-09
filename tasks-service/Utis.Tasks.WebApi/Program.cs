using Utis.Tasks.WebApi.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.WriteTo.Console()
	.Enrich.FromLogContext()
	.CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//TODO: add tracing middleware with opentel
//app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
