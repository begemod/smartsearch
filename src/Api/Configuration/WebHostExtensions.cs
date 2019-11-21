using System.Threading.Tasks;
using Api.Features.ManagementCompanies.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using Serilog;

namespace Api.Configuration
{
    public static class WebHostExtensions
    {
        public static async Task EnsureIndexCreated(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var client = scope.ServiceProvider.GetService<IElasticClient>();
                var logger = scope.ServiceProvider.GetService<ILogger<Program>>();

                await CreateIndex(client, logger);
            }
        }

        private static async Task CreateIndex(IElasticClient client, ILogger<Program> logger)
        {
            logger.LogInformation("Checking if index exists....");

            var existsResponse = client.Indices.Exists(Indices.Index(Common.Constants.Elasticsearch.DataIndexName));

            if (existsResponse.ApiCall?.Success != true)
            {
                Log.Error($"Error occured on checking of index existence. Error: {existsResponse.OriginalException?.Message }");
            }

            if (existsResponse.Exists)
            {
                Log.Information($"The index {Common.Constants.Elasticsearch.DataIndexName} already exists");
                return;
            }

            logger.LogInformation($"Try to create new index with name {Common.Constants.Elasticsearch.DataIndexName}");

            await client.Indices.CreateAsync(
                Indices.Index(Common.Constants.Elasticsearch.DataIndexName),
                c => c.Map<ManagementCompanyData>(m => m.AutoMap())
            );

            logger.LogInformation($"The index with name {Common.Constants.Elasticsearch.DataIndexName} successfully created");
        }
    }
}