using App.Domain.Entities.EmailFactory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations
{
    public class SendGridConfigConfiguration : IEntityTypeConfiguration<SendGridConfig>
    {
        public void Configure(EntityTypeBuilder<SendGridConfig> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.ApiKey)
                .HasMaxLength(500);

            // Configure relationship with EmailProviderConfigs
            builder.HasOne(e => e.EmailProviderConfig)
                .WithOne(e => e.SendGridConfig)
                .HasForeignKey<SendGridConfig>(e => e.EmailProviderConfigId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
