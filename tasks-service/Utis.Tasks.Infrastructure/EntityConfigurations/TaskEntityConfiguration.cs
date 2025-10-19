using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Utis.Tasks.Infrastructure.Entities;
using Utis.Tasks.Domain.Models;


namespace Utis.Tasks.Infrastructure.EntityConfigurations
{
	public class TaskEntityConfiguration : IEntityTypeConfiguration<TaskEntity>
	{
		public void Configure(EntityTypeBuilder<TaskEntity> builder)
		{
			builder.ToTable("tasks");
			builder.HasKey(s => s.Id);

			builder.Property(s => s.Status)
				.HasConversion<string>()
				.HasDefaultValue(TaskState.New);

			builder.HasIndex(s => s.Status);
			builder.HasIndex(s => s.DueDate);
		}
	}
}
