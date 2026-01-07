using App.Domain.Entities.EmailFactory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations
{
    public class SmtpConfigConfiguration : IEntityTypeConfiguration<SmtpConfig>
    {
        public void Configure(EntityTypeBuilder<SmtpConfig> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.SmtpServer)
                .HasMaxLength(200);
                
            builder.Property(e => e.UserName)
                .HasMaxLength(100);
                
            builder.Property(e => e.Password)
                .HasMaxLength(500);
                
            builder.Property(e => e.FromEmail)
                .HasMaxLength(100);
                
            builder.Property(e => e.FromName)
                .HasMaxLength(100);

            // Configure relationship with EmailProviderConfigs
            builder.HasOne(e => e.EmailProviderConfig)
                .WithOne(e => e.SmtpConfig)
                .HasForeignKey<SmtpConfig>(e => e.EmailProviderConfigId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
