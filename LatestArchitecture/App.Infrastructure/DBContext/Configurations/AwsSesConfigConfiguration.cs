using App.Domain.Entities.EmailFactory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations
{
    public class AwsSesConfigConfiguration : IEntityTypeConfiguration<AwsSesConfig>
    {
        public void Configure(EntityTypeBuilder<AwsSesConfig> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.AwsAccessKey)
                .HasMaxLength(100);
                
            builder.Property(e => e.AwsSecretKey)
                .HasMaxLength(100);
                
            builder.Property(e => e.AwsRegion)
                .HasMaxLength(100);

            // Configure relationship with EmailProviderConfigs
            builder.HasOne(e => e.EmailProviderConfig)
                .WithOne(e => e.AwsSesConfig)
                .HasForeignKey<AwsSesConfig>(e => e.EmailProviderConfigId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
