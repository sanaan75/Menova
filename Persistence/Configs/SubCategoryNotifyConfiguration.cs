using Entities.Menus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class SubCategoryNotifyConfiguration : IEntityTypeConfiguration<SubCategoryNotify>
{
    public void Configure(EntityTypeBuilder<SubCategoryNotify> builder)
    {
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.SubCategoryId);
    }
}