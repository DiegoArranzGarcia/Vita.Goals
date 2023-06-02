﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vita.Core.Infrastructure.Sql;
using Vita.Goals.Domain.Aggregates.Tasks;

namespace Vita.Goals.Infrastructure.Sql.Aggregates.Tasks;

public class TaskEntityConfiguration : EntityTypeConfiguration<Task>
{
    public override void Configure(EntityTypeBuilder<Task> builder)
    {
        base.Configure(builder);

        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
               .HasColumnName(nameof(Task.Title))
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(t => t.CreatedOn)
               .HasColumnType("datetimeoffset(0)")
               .IsRequired();

        builder.HasOne(x => x.AssociatedTo)
               .WithMany(x => x.Tasks)
               .IsRequired(false);

        builder.Property(x => x.Status)
               .HasColumnName("TaskStatusId")
               .HasConversion(p => p.Id,
                              p => Core.Domain.Enumeration.FromValue<TaskStatus>(p));

        builder.OwnsOne(t => t.PlannedDate);
    }
}
