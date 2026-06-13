using Entities.Menus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.UserId);

        builder.Property(i => i.Name).HasMaxLength(50);
        builder.HasIndex(i => i.Name);
    }
}