using Microsoft.EntityFrameworkCore;
using EFCore.NamingConventions;
using Serilog;

using Utis.Tasks.WebApi.Services;
using Utis.Tasks.Infrastructure;
using Utis.Tasks.Infrastructure.Repositories;
using Utis.Tasks.Domain.Interfaces;
using Utis.Tasks.Domain.Entities;

namespace Utis.Tasks.WebApi.Configuration
{
	public static class ApplicationConfig
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<StorageContext>(options =>
			{
				//if (EntityFrameworkLog) options.UseLoggerFactory(EFLoggerFactory);
				options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), 
				o => o.CommandTimeout(30)
					.SetPostgresVersion(13, 0))
				.UseSnakeCaseNamingConvention()
				.EnableSensitiveDataLogging()
				.EnableDetailedErrors();
			}, ServiceLifetime.Scoped, ServiceLifetime.Scoped);

			services.AddSingleton<Serilog.ILogger>(Log.Logger);

			// сервис для хэширования паролей
			//services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
			// сервис для генерации JWT токенов
			//services.AddScoped<IJwtService, JwtService>();

			services.AddScoped<ITaskRepository, TaskRepository>();
			//services.AddScoped<IChatRepository, ChatRepository>();
			//services.AddScoped<IMessageRepository, MessageRepository>();

			//services.AddScoped<IUnitOfWork, UnitOfWork>();

			services.AddScoped<ITaskService, TaskService>();
			//services.AddScoped<IChatService, ChatService>();
			//services.AddScoped<IMessageService, MessageService>();

			



			return services;
		}
	}
}
