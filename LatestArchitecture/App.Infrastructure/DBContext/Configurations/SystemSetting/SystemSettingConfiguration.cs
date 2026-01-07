using App.Domain.Entities.MasterSystemSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.DBContext.Configurations.SystemSetting
{
    public class SystemSettingConfiguration
    {
        public void Configure(EntityTypeBuilder<MasterSystemSetting> builder)
        {
            builder.ToTable("MasterSystemSetting");

            // Map BaseEntity.Id → SystemSettingId column
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .HasColumnName("SystemSettingId")
                   .ValueGeneratedOnAdd();

            // Map other properties / audit columns
            builder.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
            builder.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
            builder.Property(x => x.DeletedAt).HasColumnName("DeletedAt");
            builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
            builder.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
            builder.Property(x => x.DeletedBy).HasColumnName("DeletedBy");

            builder.Property(x => x.SystemSettingName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.IsActive)
               .HasDefaultValue(true)        // ✅ default true
               .ValueGeneratedOnAdd();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        }
    }
}
