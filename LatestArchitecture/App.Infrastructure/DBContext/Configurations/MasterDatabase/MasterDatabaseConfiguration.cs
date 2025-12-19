using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.DBContext.Configurations.MasterDatabase
{
    public class MasterDatabaseConfiguration : IEntityTypeConfiguration<Domain.Entities.MasterDatabase.MasterDatabase>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.MasterDatabase.MasterDatabase> builder)
        {
            builder.Property(o => o.Id)
                   .HasColumnName("DatabaseID");
        }

     
    }
}
