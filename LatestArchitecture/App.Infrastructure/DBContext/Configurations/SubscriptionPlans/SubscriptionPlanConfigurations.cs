using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations.SubscriptionPlans
{
    public class SubscriptionPlanConfigurations : IEntityTypeConfiguration<Domain.Entities.SubscriptionPlan.SubscriptionPlans>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.SubscriptionPlan.SubscriptionPlans> builder)
        {
            builder.Property(o => o.Id)
                   .HasColumnName("SubscriptionPlanID");
        }
    }
}
