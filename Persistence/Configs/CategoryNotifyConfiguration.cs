using Entities.Menus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class CategoryNotifyConfiguration : IEntityTypeConfiguration<CategoryNotify>
{
    public void Configure(EntityTypeBuilder<CategoryNotify> builder)
    {
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.CategoryId);
    }
}