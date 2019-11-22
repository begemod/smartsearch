using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Common.MultipartRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.ManagementCompanies
{
    [Route("api/mgmt")]
    public class ManagementCompaniesController : Controller
    {
        private readonly IMediator _mediator;

        public ManagementCompaniesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("import")]
        [DisableFormValueModelBinding]
        public async Task<Unit> Import(CancellationToken cancellationToken) =>
            await _mediator.Send(new Import.Command(), cancellationToken);
    }
}