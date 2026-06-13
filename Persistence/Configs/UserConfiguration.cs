using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(i => i.Id);

        builder.Property(i => i.Name).HasMaxLength(50);
        builder.HasIndex(i => i.Name);

        builder.Property(i => i.Mobile).HasMaxLength(11);
        builder.HasIndex(i => i.Mobile);
        
        builder.Property(i => i.Username).HasMaxLength(30);
        builder.HasIndex(i => i.Username);
        
        builder.Property(i => i.Name).HasMaxLength(50);
        builder.HasIndex(i => i.Name);
        
        //builder.Property(i => i.Note).HasColumnType("text");
        //builder.HasOne(i => i.Owner).WithOne(i => i.Land).HasForeignKey<Land>(i => i.OwnerId);
    }
}