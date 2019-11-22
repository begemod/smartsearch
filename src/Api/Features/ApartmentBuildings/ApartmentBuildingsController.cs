using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Common.MultipartRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.ApartmentBuildings
{
    [Route("api/prop")]
    public class ApartmentBuildingsController : Controller
    {
        private readonly IMediator _mediator;

        public ApartmentBuildingsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("import")]
        [DisableFormValueModelBinding]
        public async Task<Unit> Import(CancellationToken cancellationToken) =>
            await _mediator.Send(new Import.Command(), cancellationToken);
    }
}