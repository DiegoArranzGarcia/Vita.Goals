using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vita.Core.Infrastructure.Sql;
using Vita.Goals.Domain.Aggregates.Categories;
using Vita.Goals.Domain.ValueObjects;

namespace Vita.Goals.Infrastructure.Sql.Aggregates.Categories
{
    public class CategoriesConfiguration : EntityTypeConfiguration<Category>
    {
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            base.Configure(builder);

            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .HasColumnName(nameof(Category.Name))
                   .IsRequired()
                   .HasMaxLength(256);

            builder.Property(x => x.Color)
                   .HasConversion(x => x.Color, x => new HexColor(x))
                   .HasColumnName(nameof(Category.Color))
                   .IsRequired();

            builder.Property(c => c.CreatedBy)
                   .IsRequired();
        }
    }
}
