using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Common.MultipartRequest;
using Api.Features.ManagementCompanies.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;

namespace Api.Features.ManagementCompanies.Import
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

            _logger.LogInformation("Start import of Management Companies data");

            using (var memoryStream = new MemoryStream())
            using (var streamReader = new StreamReader(memoryStream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                await request.FetchFormData(new Stream[] { memoryStream }, cancellationToken);

                var serializer = new JsonSerializer();

                var companiesData = serializer.Deserialize<ManagementCompanyContainer[]>(jsonReader);

                var companies = companiesData.Where(c => c.Data != null)
                                             .Select(c => c.Data)
                                             .Distinct(new ManagementCompanyEqualityComparer());

                if (!companies.Any())
                {
                    _logger.LogInformation("No valid companies data found in file...");
                    return Unit.Value;
                }

                var response = await _elasticClient.BulkAsync(
                                   b => b.Index(Common.Constants.Elasticsearch.IndexName.ManagementCompanies)
                                                .IndexMany(companies),
                                   cancellationToken);

                _logger.LogInformation($"{response.Items.Count} Management Company items successfully indexed");
            }

            return Unit.Value;
        }
    }
}