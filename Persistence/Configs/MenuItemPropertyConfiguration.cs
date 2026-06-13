using Entities.Menus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class MenuItemPropertyConfiguration : IEntityTypeConfiguration<MenuItemProperty>
{
    public void Configure(EntityTypeBuilder<MenuItemProperty> builder)
    {
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.MenuItemId);
    }
}