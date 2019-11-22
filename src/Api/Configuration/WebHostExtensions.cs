using System;
using System.Threading.Tasks;
using Api.Features.ApartmentBuildings.Domain;
using Api.Features.ManagementCompanies.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Api.Configuration
{
    public static class WebHostExtensions
    {
        public static async Task EnsureIndexCreatedAsync(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var client = scope.ServiceProvider.GetService<IElasticClient>();
                var logger = scope.ServiceProvider.GetService<ILogger<Program>>();

                await EnsureSpecificIndexCreatedAsync(
                    Common.Constants.Elasticsearch.IndexName.ManagementCompanies,
                    client,
                    CreateManagementCompaniesIndexAsync,
                    logger);

                await EnsureSpecificIndexCreatedAsync(
                    Common.Constants.Elasticsearch.IndexName.ApartmentBuildings,
                    client,
                    CreateApartmentBuildingsIndexAsync,
                    logger);
            }
        }

        private static async Task EnsureSpecificIndexCreatedAsync(
            string indexName,
            IElasticClient client,
            Func<string, IElasticClient, ILogger, Task> indexCreator,
            ILogger logger)
        {
            logger.LogInformation($"Checking if {indexName} index exists....");

            var existsResponse = await client.Indices.ExistsAsync(indexName);

            if (existsResponse.ApiCall?.Success != true)
            {
                Log.Error($"Error occured on checking of index existence. Error: {existsResponse.OriginalException?.Message }");
            }

            if (existsResponse.Exists)
            {
                Log.Information($"The index {indexName} already exists");
                return;
            }

            logger.LogInformation($"Try to create new index with name {indexName}");

            await indexCreator(indexName, client, logger);

            logger.LogInformation($"The index with name {indexName} successfully created");
        }

        private static async Task CreateManagementCompaniesIndexAsync(string indexName, IElasticClient client, ILogger logger)
        {
            await client.Indices.CreateAsync(indexName,
                            c => c.Settings(s =>
                                    s.Analysis(a =>
                                                   a.Analyzers(aa =>
                                                                   aa.Stop("stw", st => st.StopWords("_english_"))
                                                              )
                                              )
                                    )
                                     .Map<ManagementCompany>(
                                                    x => x.AutoMap()
                                                          .Properties(
                                                            p => p.Text(n => n
                                                                            .Name(m => m.Name)
                                                                            .Analyzer("stw"))
                                                                  .Keyword(n => n.Name(m => m.Market))
                                                                  .Keyword(n => n.Name(m => m.State))
                                                        )
                                                    )
            );
        }

        private static async Task CreateApartmentBuildingsIndexAsync(string indexName, IElasticClient client, ILogger logger)
        {
            await client.Indices.CreateAsync(indexName,
                c => c.Settings(s =>
                                    s.Analysis(a =>
                                                   a.Analyzers(aa =>
                                                                   aa.Stop("stw", st => st.StopWords("_english_"))
                                                   )
                                            )
                                    )
                             .Map<ApartmentBuilding>(x => x
                                                         .AutoMap()
                                                         .Properties(p => p
                                                                         .Text(n => n
                                                                                   .Name(m => m.Name)
                                                                                   .Analyzer("stw")
                                                                               )
                                                                         .Text(fn => fn
                                                                                   .Name(m => m.FormerName)
                                                                                   .Analyzer("stw"))
                                                                         .Text(sa => sa
                                                                                   .Name(m => m.StreetAddress)
                                                                                   .Analyzer("stw"))
                                                                         .Text(ct => ct
                                                                                   .Name(m => m.City)
                                                                                   .Analyzer("stw"))
                                                                         .Keyword(n => n.Name(m => m.Market))
                                                                         .Keyword(n => n.Name(m => m.State))
                                                                     )
                                                     )
                    );
        }
    }
}