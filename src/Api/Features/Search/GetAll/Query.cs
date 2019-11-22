using MediatR;

namespace Api.Features.Search.GetAll
{
    public class Query : IRequest<QueryResult[]>
    {
        public string Phrase { get; set; }

        public string Markets { get; set; }

        public int Limit { get; set; }
    }
}