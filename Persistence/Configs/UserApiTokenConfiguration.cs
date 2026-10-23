using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class UserApiTokenConfiguration : IEntityTypeConfiguration<UserApiToken>
{
    public void Configure(EntityTypeBuilder<UserApiToken> builder)
    {
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.UserId);
        builder.HasIndex(i => i.Token);
    }
}
