using Yarp.ReverseProxy.Configuration;

namespace App.Gateway.Api.Configuration
{
    public static class ReverseProxyExtensions
    {
        public static IServiceCollection AddCustomReverseProxy(this IServiceCollection services)
        {
            _ = services.AddReverseProxy()
                .LoadFromMemory(
                    [
                        // Company Registration route with path transform
                        new()
                        {
                            RouteId = "companyregistration_route",
                            ClusterId = "companyregistration_cluster",
                            Match = new RouteMatch
                            {
                                Path = "/companyregistration/{**catch-all}"
                            },
                            Transforms = new[]
                            {
                                new Dictionary<string, string>
                                {
                                    { "PathRemovePrefix", "/companyregistration" }
                                }
                            }
                        }
                    ],
                    [
                        new ClusterConfig
                        {
                            ClusterId = "companyregistration_cluster",
                            Destinations = new Dictionary<string, DestinationConfig>
                            {
                                ["destination1"] = new DestinationConfig
                                {
                                    Address = "https://localhost:7110/"
                                }
                            }
                        }
                    ]
                );
            return services;
        }
    }
}
