using App.Domain.Entities.EmailFactory;
using Microsoft.EntityFrameworkCore;
using极EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations
{
    public class SendGridConfigConfiguration : IEntityTypeConfiguration<SendGridConfig>
    {
        public void Configure(EntityTypeBuilder<SendGridConfig> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.ApiKey)
                .HasLength(500);

            // Configure relationship with EmailProviderConfig
            builder.HasOne(e => e.EmailProviderConfig)
                .WithOne(e => e.SendGridConfig)
                .HasForeignKey<SendGridConfig>(e => e.EmailProviderConfigId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
