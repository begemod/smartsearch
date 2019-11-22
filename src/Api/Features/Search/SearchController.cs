using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Search
{
    [Route("api/search")]
    public class SearchController : Controller
    {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string phrase,
            [FromQuery] string markets,
            [FromQuery] int? limit, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetAll.Query
            {
                Phrase = phrase,
                Markets = markets,
                Limit = limit ?? 25
            }, cancellationToken));
        }
    }
}