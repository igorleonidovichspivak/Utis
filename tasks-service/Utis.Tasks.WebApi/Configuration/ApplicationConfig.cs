using Microsoft.Extensions.Configuration;
//using EFCore.NamingConventions;
//using Just.Domain.Entities;
//using Just.Domain.Interfaces.DataStorage;
//using Just.Infrastructure;
//using Just.Infrastructure.Repositories;
//using Just.WebApi;
//using Just.WebApi.Services;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using NodaTime;
using Serilog;



namespace Utis.Tasks.WebApi.Configuration
{
	public static class ApplicationConfig
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
		{
			//services.AddDbContext<StorageContext>(options =>
			//{
			//	//if (EntityFrameworkLog) options.UseLoggerFactory(EFLoggerFactory);
			//	options.UseNpgsql(configuration.GetConnectionString("StorageContext"), 
			//	o => o.CommandTimeout(30)
			//		.SetPostgresVersion(13, 0)
			//				//.UseNodaTime()
			//		).UseSnakeCaseNamingConvention();
			//}, ServiceLifetime.Transient, ServiceLifetime.Transient);

			services.AddSingleton<Serilog.ILogger>(Log.Logger);

			// сервис для хэширования паролей
			//services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
			// сервис для генерации JWT токенов
			//services.AddScoped<IJwtService, JwtService>();

			//services.AddScoped<IUserRepository, UserRepository>();
			//services.AddScoped<IChatRepository, ChatRepository>();
			//services.AddScoped<IMessageRepository, MessageRepository>();

			//services.AddScoped<IUnitOfWork, UnitOfWork>();

			//services.AddScoped<IUserService, UserService>();
			//services.AddScoped<IChatService, ChatService>();
			//services.AddScoped<IMessageService, MessageService>();

			



			return services;
		}
	}
}
