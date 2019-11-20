using Api.Common.Constants;
using Microsoft.Extensions.Configuration;

namespace Api.Configuration
{
    public static class ConfigurationExtensions
    {
        public static string GetAwsElasticsearchAccessKey(this IConfiguration configuration) =>
            configuration?[EnvironmentVariableNames.AwsElasticsearchAccessKey]?.Trim() ?? string.Empty;

        public static string GetAwsElasticsearchSecretKey(this IConfiguration configuration) =>
            configuration?[EnvironmentVariableNames.AwsElasticsearchSecretKey]?.Trim() ?? string.Empty;

        public static string GetAwsElasticsearchRegionEndpointName(this IConfiguration configuration) =>
            configuration?[EnvironmentVariableNames.AwsRegionEndpoint]?.Trim() ?? string.Empty;

        public static string GetAwsElasticsearchConnectionString(this IConfiguration configuration) =>
            configuration?[EnvironmentVariableNames.AwsElasticsearchConnectionString]?.Trim() ?? string.Empty;
    }
}