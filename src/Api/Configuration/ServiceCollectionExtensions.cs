using System;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Api.Pipeline;
using Elasticsearch.Net;
using Elasticsearch.Net.Aws;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Api.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            AssemblyScanner.FindValidatorsInAssembly(typeof(Startup).Assembly)
                .ForEach(v => services.AddScoped(v.InterfaceType, v.ValidatorType));

            services.AddMediatR(typeof(Startup));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TimerBehavior<,>));

            return services;
        }

        public static IServiceCollection AddElasticsearch(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var awsCredentials = new BasicAWSCredentials(
                configuration.GetAwsElasticsearchAccessKey(),
                configuration.GetAwsElasticsearchSecretKey());

            var httpConnection = new AwsHttpConnection(
                new AWSOptions
                {
                    Credentials = awsCredentials,
                    Region = RegionEndpoint.GetBySystemName(configuration.GetAwsElasticsearchRegionEndpointName())
                });

            var pool = new SingleNodeConnectionPool(new Uri(configuration.GetAwsElasticsearchConnectionString()));

            var config = new ConnectionSettings(pool, httpConnection)
                                            .ThrowExceptions()
                                            .DefaultIndex(Common.Constants.Elasticsearch.DataIndexName);

            var client = new ElasticClient(config);

            services.AddSingleton<IElasticClient>(client);

            return services;
        }
    }
}