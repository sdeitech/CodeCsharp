using App.Application.Interfaces.Services.Email;
using App.Domain.Entities;
using App.Domain.Entities.EmailFactory;
using System;

namespace App.Application.Service.EmailProviders
{
    public class EmailProviderFactory
    {
        public static IEmailProvider CreateProvider(EmailProviderConfigs config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (config.EmailProviderType == null || string.IsNullOrEmpty(config.EmailProviderType.Name))
                throw new ArgumentException("Email provider type is not loaded or has no name");

            var providerName = config.EmailProviderType.Name.ToUpperInvariant();

            return providerName switch
            {
                "SMTP" => new SmtpEmailProvider(config.SmtpConfig),
                "AWSSES" => new AwsSesEmailProvider(config.AwsSesConfig),
                "SENDGRID" => new SendGridEmailProvider(config.SendGridConfig),
                "AZURE" => new AzureEmailProvider(config.AzureConfig),
                _ => throw new NotSupportedException($"Email provider type {config.EmailProviderType.Name} is not supported")
            };
        }
    }
}
