using MediatR;
using Microsoft.Extensions.Logging;
using Revo.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Revo.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : 
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : Result
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // get the request name 
            var reqName = typeof(TRequest).Name;
            var timeStamp = DateTime.Now;

            // log the request name and timestamp when it start
            _logger.LogInformation("Handling request {RequestName} at {TimeStamp}", reqName, timeStamp);
            
            var stopwatch = Stopwatch.StartNew();
            
            var respo = await next();

            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds; // get the elapsed time in milliseconds

            if (respo.IsFailure)
            {
                _logger.LogWarning(
                     "Request {RequestName} failed in {ElapsedMilliseconds}ms with Error Code: {ErrorCode}, Message: {ErrorMessage}",
                     reqName,
                     elapsedMs,
                     respo.Error.Code,
                     respo.Error.Message);
                return respo;

            }
            if (elapsedMs > 500)
            {
                _logger.LogWarning(
                     "Long Running Request: {RequestName} completed successfully, but took {ElapsedMilliseconds}ms",
                     reqName,
                     elapsedMs);
                return respo;
            }
            else
            {
                _logger.LogInformation(
                    "Completed request {RequestName} successfully in {ElapsedMilliseconds}ms",
                    reqName,
                    elapsedMs);
            }

                return respo;

            }
    }
}
