using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.DBContext.Configurations.MasterData
{
    public class MasterStateConfiguration : IEntityTypeConfiguration<Domain.Entities.MasterData.MasterState>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.MasterData.MasterState> builder)
        {
            builder.Property(o => o.Id)
                   .HasColumnName("StateID");
        }
    }
}
