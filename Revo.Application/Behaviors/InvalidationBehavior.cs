using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Revo.Application.Abstraction.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Behaviors
{
    public class InvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ICacheInvalidatorCommand
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<InvalidationBehavior<TRequest, TResponse>> _logger;
        public InvalidationBehavior(IDistributedCache cache, ILogger<InvalidationBehavior<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            var isSuccessProperty = typeof(TResponse).GetProperty("IsSuccess");
            if (isSuccessProperty != null)
            {
                var isSuccess = (bool)isSuccessProperty.GetValue(response)!;
                if (!isSuccess)
                    return response;
            }

            try
            {
                foreach (var group in request.CacheGroupsToClear)
                {
                    var groupVersionKey = $"Version_{group}";
                    await _cache.RemoveAsync(groupVersionKey, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to invalidate Redis cache.");
            }

            return response;
        }
    }
}