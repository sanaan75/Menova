using Entities.Menus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class SubCategoryConfiguration : IEntityTypeConfiguration<SubCategory>
{
    public void Configure(EntityTypeBuilder<SubCategory> builder)
    {
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.CategoryId);

        builder.Property(i => i.Name).HasMaxLength(50);
        builder.HasIndex(i => i.Name);
    }
}