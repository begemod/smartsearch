using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Features.ManagementCompanies.Domain;
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

            var managementCompaniesSearchResult = await SearchManagementCompaniesAsync(
                                                      queryPhrase,
                                                      markets,
                                                      query.Limit,
                                                      cancellationToken);

            return managementCompaniesSearchResult.Select(r => new QueryResult
            {
                Id = r.Id,
                Name = r.Name,
                Type = "mgmt",
                State = r.State,
                Market = r.Market
            }).ToArray();
        }

        private async Task<IReadOnlyCollection<ManagementCompany>> SearchManagementCompaniesAsync(
            string queryPhrase,
            IReadOnlyCollection<string> markets,
            int limit,
            CancellationToken cancellationToken)
        {
            var result = await _elasticClient.SearchAsync<ManagementCompanies.Domain.ManagementCompany>(
                c => c.Index(Common.Constants.Elasticsearch.IndexName.ManagementCompanies)
                                        .Query(q => q.Bool(b =>
                                                               b.Must(m =>
                                                                          m.Match(mm =>
                                                                                      mm.Field(f => f.Name)
                                                                                          .Query(queryPhrase)
                                                                          ))
                                                                   .Filter(f =>
                                                                               f.Terms(t =>
                                                                                           t.Field(tf => tf.Market)
                                                                                               .Terms(markets)
                                                                               )
                                                                   )
                                               )
                                        )
                    .Take(limit)
                , cancellationToken);

            return result.Documents;
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