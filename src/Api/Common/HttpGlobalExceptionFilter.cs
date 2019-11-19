using System;
using System.Linq;
using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Api.Common
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);

            var errorJson = new JsonErrorResponse { Message = context.Exception.Message };

            if (_env.IsDevelopment())
            {
                errorJson.DeveloperMessage = context.Exception.StackTrace;
            }

            switch (context.Exception)
            {
                case ValidationException e:

                    errorJson.Errors = e.Errors == null ? Array.Empty<ErrorEntry>() : e.Errors.Select(v => new ErrorEntry
                    {
                        Code = int.TryParse(v.ErrorCode, out var res) ? res : 0,
                        Message = v.ErrorMessage
                    }).ToArray();

                    context.Result = new UnprocessableEntityObjectResult(errorJson);
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    break;

                default:
                    context.Result = new ObjectResult(errorJson);
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    break;
            }
        }
    }
}