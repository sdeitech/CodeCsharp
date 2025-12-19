using App.Domain.Entities.MasterTimeZone;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.DBContext.Configurations.MasterTimeZone
{
    public class MasterTimeZonesConfiguration : IEntityTypeConfiguration<MasterTimeZones>
    {
        public void Configure(EntityTypeBuilder<MasterTimeZones> entity)
        {
            // Ignore BaseEntity fields not present in DB
            entity.Ignore(e => e.Id);
            entity.Ignore(e => e.CreatedBy);
            entity.Ignore(e => e.UpdatedBy);
            entity.Ignore(e => e.DeletedAt);
            entity.Ignore(e => e.DeletedBy);

            // Optional: set table name if needed
            entity.ToTable("MasterTimeZones");
        }
    }
}
