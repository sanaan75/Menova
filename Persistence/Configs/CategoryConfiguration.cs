using Entities.Menus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.MenuId);

        builder.Property(i => i.Name).HasMaxLength(50);
        builder.HasIndex(i => i.Name);
    }
}