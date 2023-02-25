using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vita.Goals.Domain.Aggregates.Goals;

namespace Vita.Goals.Infrastructure.Sql.Aggregates.Goals;

public class GoalStatusSafeEnumConfiguration : IEntityTypeConfiguration<GoalStatus>
{
    public void Configure(EntityTypeBuilder<GoalStatus> builder)
    {
        builder.ToTable("GoalStatus");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
               .ValueGeneratedNever()
               .IsRequired();

        builder.Property(o => o.Name)
               .HasMaxLength(200)
               .IsRequired();
    }
}
