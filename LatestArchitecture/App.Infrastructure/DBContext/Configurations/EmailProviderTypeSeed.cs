using App.Domain.Entities.EmailFactory;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.DBContext.Configurations
{
    public static class EmailProviderTypeSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailProviderType>().HasData(
                new EmailProviderType
                {
                    Id = 1,
                    Name = "SMTP",
                    Description = "Standard SMTP email provider",
                    IsActive = true
                },
                new EmailProviderType
                {
                    Id = 2,
                    Name = "AWSSES",
                    Description = "Amazon Web Services Simple Email Service",
                    IsActive = true
                },
                new EmailProviderType
                {
                    Id = 3,
                    Name = "SendGrid",
                    Description = "SendGrid email delivery service",
                    IsActive = true
                },
                new EmailProviderType
                {
                    Id = 4,
                    Name = "Azure",
                    Description = "Microsoft Azure Communication Services Email",
                    IsActive = true
                }
            );
        }
    }
}
