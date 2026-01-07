using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.DBContext.Configurations.MasterData
{
    public class MasterDataConfiguration : IEntityTypeConfiguration<Domain.Entities.MasterData.MasterCountry>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.MasterData.MasterCountry> builder)
        {
            builder.Property(o => o.Id)
                   .HasColumnName("CountryID");
        }
    }
}
