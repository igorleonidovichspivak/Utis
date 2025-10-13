using Serilog;
using Utis.Processing.Service;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.WriteTo.Console()
	.Enrich.FromLogContext()
	.CreateLogger();

builder.Logging.AddSerilog();

builder.Services.AddHostedService<MessageConsumer>();

builder.Services.AddSingleton<IRabbitMQConsumerService, RabbitMQConsumerService>();

var host = builder.Build();
host.Run();
