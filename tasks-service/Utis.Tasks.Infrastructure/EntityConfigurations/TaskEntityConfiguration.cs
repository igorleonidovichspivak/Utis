using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utis.Tasks.Domain.Entities;

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
