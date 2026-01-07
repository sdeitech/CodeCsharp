using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using App.Application.Interfaces.Services.Email;
using App.Domain.Entities.EmailFactory;

namespace App.Application.Service.EmailProviders
{
    public class AwsSesEmailProvider : IEmailProvider
    {
        private readonly AwsSesConfig _config;

        public AwsSesEmailProvider(AwsSesConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Resolve region
            var region = RegionEndpoint.GetBySystemName(_config.AwsRegion);

            // Create SES client (can also be injected via DI if reused globally)
            using var client = new AmazonSimpleEmailServiceClient(
                _config.AwsAccessKey,
                _config.AwsSecretKey,
                region
            );

            // Build request
            var sendRequest = new SendEmailRequest
            {
               
                Destination = new Destination
                {
                    ToAddresses = new List<string> { toEmail }
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = body
                        },
                        Text = new Content
                        {
                            Charset = "UTF-8",
                            Data = StripHtmlTags(body) // fallback plain text
                        }
                    }
                }
            };

            // Send email
            var response = await client.SendEmailAsync(sendRequest);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new InvalidOperationException(
                    $"AWS SES send failed with status: {response.HttpStatusCode}"
                );
            }
        }

        private string StripHtmlTags(string html)
        {
            return string.IsNullOrWhiteSpace(html)
                ? string.Empty
                : System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
        }
    }
}
