using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vita.Goals.Domain.Aggregates.Tasks;

namespace Vita.Goals.Infrastructure.Sql.Aggregates.Tasks;

public class TaskStatusSafeEnumConfiguration : IEntityTypeConfiguration<TaskStatus>
{
    public void Configure(EntityTypeBuilder<TaskStatus> builder)
    {
        builder.ToTable("TaskStatus");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
               .ValueGeneratedNever()
               .IsRequired();

        builder.Property(o => o.Name)
               .HasMaxLength(200)
               .IsRequired();
    }
}
