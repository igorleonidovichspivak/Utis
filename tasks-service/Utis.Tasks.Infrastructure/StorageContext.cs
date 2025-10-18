using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.Common;
using Utis.Tasks.Domain.Models;
using Utis.Tasks.Infrastructure.EntityConfigurations;
using Utis.Tasks.Infrastructure.Entities;

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

	

}
