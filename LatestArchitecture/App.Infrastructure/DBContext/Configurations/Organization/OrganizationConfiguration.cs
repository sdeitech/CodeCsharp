using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.DBContext.Configurations.Organization
{
    public class OrganizationConfiguration : IEntityTypeConfiguration<Domain.Entities.Organization.Organization>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Organization.Organization> builder)
        {
            builder.Property(o => o.Id)
                   .HasColumnName("OrganizationID");
        }
    }
}
