using Entities.Menus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.UserId);
        builder.HasIndex(i => i.SubCategoryId);

        builder.Property(i => i.Name).HasMaxLength(50);
        builder.HasIndex(i => i.Name);
    }
}