using Microsoft.EntityFrameworkCore;
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

        builder.Property<int>("_taskStatusId")
               .UsePropertyAccessMode(PropertyAccessMode.Field)
               .HasColumnName("TaskStatusId")
               .IsRequired();

        builder.Property(t => t.CreatedOn)
               .HasColumnType("datetimeoffset(0)")
               .IsRequired();

        builder.HasOne(x => x.AssociatedTo)
               .WithMany(x => x.Tasks)
               .IsRequired(false);

        builder.HasOne(t => t.TaskStatus)
               .WithMany()
               .HasForeignKey("_taskStatusId");

        builder.OwnsOne(t => t.PlannedDate);
    }
}
