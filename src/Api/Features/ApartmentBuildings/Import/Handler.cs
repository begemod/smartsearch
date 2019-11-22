using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Common.MultipartRequest;
using Api.Features.ApartmentBuildings.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;

namespace Api.Features.ApartmentBuildings.Import
{
    public class Handler : IRequestHandler<Command, Unit>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<Handler> _logger;
        private readonly IElasticClient _elasticClient;

        public Handler(IHttpContextAccessor httpContextAccessor, ILogger<Handler> logger, IElasticClient elasticClient)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
        }

        public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
        {
            var request = _httpContextAccessor.HttpContext.Request;

            _logger.LogInformation("Start import of Apartment Buildings data");

            using (var memoryStream = new MemoryStream())
            using (var streamReader = new StreamReader(memoryStream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                await request.FetchFormData(new Stream[] { memoryStream }, cancellationToken);

                var serializer = new JsonSerializer();

                var buildingData = serializer.Deserialize<ApartmentBuildingContainer[]>(jsonReader);

                // get rid of duplicates
                var buildings = buildingData.Where(c => c.Property != null)
                                            .Select(c => c.Property)
                                            .Distinct(new ApartmentBuildingEqualityComparer());

                if (!buildings.Any())
                {
                    _logger.LogInformation("No valid apartment buildings data found in file...");
                    return Unit.Value;
                }

                var response = await _elasticClient.BulkAsync(b => b
                                                     .Index(Common.Constants.Elasticsearch.IndexName.ApartmentBuildings)
                                                     .IndexMany(buildings),
                                                 cancellationToken);

                _logger.LogInformation($"{response.Items.Count} Apartment Building items successfully indexed");
            }

            return Unit.Value;
        }
    }
}