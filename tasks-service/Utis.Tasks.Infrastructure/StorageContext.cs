using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.Common;
using Utis.Tasks.Domain.Entities;
using Utis.Tasks.Infrastructure.EntityConfigurations;

namespace Utis.Tasks.Infrastructure
{
	public class StorageContext: DbContext
	{
		public DbSet<TaskEntity> Tasks { get; set; }
		public StorageContext() { }
		public StorageContext(DbContextOptions<StorageContext> options) : base(options) { }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new TaskEntityConfiguration());

			base.OnModelCreating(modelBuilder);
		}
	}

	//public class StorageContextFactory : IDesignTimeDbContextFactory<StorageContext>
	//{
	//	public StorageContext CreateDbContext(string[] args)
	//	{
	//		var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development" ?? "Docker";
						

	//		var configuration = new ConfigurationBuilder()
	//			.SetBasePath(Directory.GetCurrentDirectory())
	//			.AddJsonFile("appsettings.json")
	//			.AddJsonFile($"appsettings.{environment}.json", true)
	//			.Build();

	//		var optionsBuilder = new DbContextOptionsBuilder<StorageContext>();
	//		var connectionString = configuration.GetConnectionString("DefaultConnection");

	//		optionsBuilder.UseNpgsql(connectionString);

	//		return new StorageContext(optionsBuilder.Options);
	//	}
	//}

}
