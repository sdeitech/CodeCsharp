using App.Domain.Entities.EmailFactory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations
{
    public class AzureConfigConfiguration : IEntityTypeConfiguration<AzureConfig>
    {
        public void Configure(EntityTypeBuilder<AzureConfig> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.ConnectionString)
                .HasMaxLength(500);
                
            builder.Property(e => e.SenderAddress)
                .HasMaxLength(100);

            // Configure relationship with EmailProviderConfigs
            builder.HasOne(e => e.EmailProviderConfig)
                .WithOne(e => e.AzureConfig)
                .HasForeignKey<AzureConfig>(e => e.EmailProviderConfigId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
