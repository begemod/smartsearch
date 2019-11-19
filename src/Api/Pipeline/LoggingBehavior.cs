using System.Threading;
using System.Threading.Tasks;
using Api.Common.Constants;
using MediatR;
using Serilog;
using Serilog.Context;

namespace Api.Pipeline
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            // push property to Serilog context to output it in logs
            using (LogContext.PushProperty(LogConstants.MediatRRequestType, typeof(TRequest).FullName))
            {
                Log.Information("Executing command: {@request}", request);

                return await next();
            }
        }
    }
}