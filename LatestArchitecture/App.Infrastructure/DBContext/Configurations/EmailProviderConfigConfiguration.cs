using App.Domain.Entities.EmailFactory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.DBContext.Configurations
{
    public class EmailProviderConfigConfiguration : IEntityTypeConfiguration<EmailProviderConfigs>
    {
        public void Configure(EntityTypeBuilder<EmailProviderConfigs> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.OrganizationId)
                .IsRequired();
                
            builder.Property(e => e.EmailProviderTypeId)
                .IsRequired();
                
            builder.Property(e => e.TemplatesPath)
                .HasMaxLength(500);

            // Configure relationship with EmailProviderType
            builder.HasOne(e => e.EmailProviderType)
                .WithMany(e => e.EmailProviderConfigs)
                .HasForeignKey(e => e.EmailProviderTypeId)
                .OnDelete(DeleteBehavior.Restrict);

   

            // Configure optional one-to-one relationships with provider-specific configs
            builder.HasOne(e => e.SmtpConfig)
                .WithOne(e => e.EmailProviderConfig)
                .HasForeignKey<SmtpConfig>(e => e.EmailProviderConfigId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne(e => e.AwsSesConfig)
                .WithOne(e => e.EmailProviderConfig)
                .HasForeignKey<AwsSesConfig>(e => e.EmailProviderConfigId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne(e => e.SendGridConfig)
                .WithOne(e => e.EmailProviderConfig)
                .HasForeignKey<SendGridConfig>(e => e.EmailProviderConfigId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne(e => e.AzureConfig)
                .WithOne(e => e.EmailProviderConfig)
                .HasForeignKey<AzureConfig>(e => e.EmailProviderConfigId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
