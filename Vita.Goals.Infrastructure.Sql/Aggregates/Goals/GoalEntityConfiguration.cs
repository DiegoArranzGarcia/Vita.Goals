using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vita.Core.Infrastructure.Sql;
using Vita.Goals.Domain.Aggregates.Goals;

namespace Vita.Goals.Infrastructure.Sql.Aggregates.Goals
{
    public class GoalEntityConfiguration : EntityTypeConfiguration<Goal>
    {
        public override void Configure(EntityTypeBuilder<Goal> builder)
        {
            base.Configure(builder);

            builder.ToTable("Goals");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Title)
                   .HasColumnName(nameof(Goal.Title))
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(g => g.CreatedBy)
                   .HasColumnName(nameof(Goal.CreatedBy))
                   .IsRequired();

            builder.Property<int>("_goalStatusId")
                   .HasColumnName("GoalStatusId")
                   .IsRequired();

            builder.Property(g => g.CreatedOn)
                   .HasColumnType("datetimeoffset(0)")
                   .IsRequired();

            builder.Property(g => g.Description)
                   .IsRequired(false)
                   .HasMaxLength(255);

            builder.HasMany(g => g.Tasks)
                   .WithOne(t => t.AssociatedTo)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(g => g.GoalStatus)
                   .WithMany()
                   .HasForeignKey("_goalStatusId");

            builder.OwnsOne(g => g.AimDate);
        }
    }
}
