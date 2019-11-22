using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Nest;

namespace Api.Features.Search.GetAll
{
    public class Handler : IRequestHandler<Query, QueryResult[]>
    {
        private readonly IElasticClient _elasticClient;

        public Handler(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
        }

        public async Task<QueryResult[]> Handle(Query query, CancellationToken cancellationToken)
        {
            var queryPhrase = NormalizePhrase(query);

            var markets = FetchMarketsFilter(query);

            var result = await _elasticClient.SearchAsync<ConsolidatedData>(c =>
                                                             c.Index(Indices.Index(
                                                                 Common.Constants.Elasticsearch.IndexName.ManagementCompanies,
                                                                 Common.Constants.Elasticsearch.IndexName.ApartmentBuildings))
                                                                 .Query(q => q
                                                                            .Bool(b =>
                                                                                    b.Must(m => m.MultiMatch(mm =>
                                                                                                    mm.Fields(f =>
                                                                                                              f.Field(x => x.Name, 20)
                                                                                                               .Field(x => x.FormerName, 15)
                                                                                                               .Field(x => x.StreetAddress, 10)
                                                                                                               .Field(x => x.City, 5)
                                                                                                              )
                                                                                                    .Query(queryPhrase)
                                                                                                )
                                                                                            )
                                                                           .Filter(f =>
                                                                                       f.Terms(t =>
                                                                                                   t.Field(tf => tf.Market)
                                                                                                    .Terms(markets)
                                                                                       )
                                                                                    )
                                                                            )
                                                   )
                                            .Take(query.Limit)
                          , cancellationToken);

            return result.Hits.Select(h => new QueryResult
            {
                Id = h.Source.Id,
                Name = h.Source.Name,
                Type = h.Index.Equals(Common.Constants.Elasticsearch.IndexName.ManagementCompanies, StringComparison.OrdinalIgnoreCase) ? "mgmt" : "prop",
                Market = h.Source.Market,
                State = h.Source.State
            }).ToArray();
        }

        private List<string> FetchMarketsFilter(Query query)
        {
            var markets = new List<string>();

            if (!string.IsNullOrWhiteSpace(query.Markets))
            {
                markets.AddRange(
                    query.Markets.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                          .Where(m => !string.IsNullOrWhiteSpace(m))
                                          .Distinct());
            }

            return markets;
        }

        private string NormalizePhrase(Query query)
        {
            var queryPhrase = query.Phrase.AsSpan() // get rid of extra strings allocation
                                   .Trim()
                                   .Trim('\'')
                                   .Trim('\"');

            return queryPhrase.ToString();
        }
    }
}