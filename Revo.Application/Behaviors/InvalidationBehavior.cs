using MediatR;
using Microsoft.Extensions.Caching.Distributed;
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
        public InvalidationBehavior(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // 1 - Execute the next handler in the pipeline which is handler to save in the database 
            var response = await next();

            // 2 - Clear the cache keys after the database operation is successful
            foreach (var key in request.CacheKeysToClear)
                await _cache.RemoveAsync(key, cancellationToken);

            return response;

        }
    }
}
