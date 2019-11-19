using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;
using StackExchange.Profiling;

namespace Api.Pipeline
{
    public class TimerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var profiler = MiniProfiler.StartNew("commandTimings");

            var result = await next();

            await profiler.StopAsync();

            Log.Information("Command {CommandName} took {Duration} ms ", typeof(TRequest).FullName, profiler.DurationMilliseconds);

            return result;
        }
    }
}