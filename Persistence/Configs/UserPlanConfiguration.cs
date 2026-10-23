using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class UserPlanConfiguration : IEntityTypeConfiguration<UserPlan>
{
    public void Configure(EntityTypeBuilder<UserPlan> builder)
    {
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.UserId);
        builder.HasIndex(i => i.PlanId);
    }
}
