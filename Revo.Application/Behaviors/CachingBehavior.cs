using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Revo.Application.Abstraction.Caching;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Revo.Application.Abstraction.Messaging;

namespace Revo.Application.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IQuery<TResponse>, ICacheableQuery
    {
        private readonly IDistributedCache _cache;
        public CachingBehavior(IDistributedCache cache)
        {
            this._cache = cache;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // 1 - When we conver Result to JSON, we want to ignore null values and be case insensitive when deserializing
            // to save space of Redis and avoid issues with case sensitivity when deserializing
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            // 2 - If the request does not have a cache key, we will just call the next handler in the pipeline
            if (string.IsNullOrEmpty(request.CacheKey))
                return await next();

            // 3 - If the request has a cache key, we will try to get the cached response from Redis
            var cachedResponse = await _cache.GetStringAsync(request.CacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedResponse))
                return JsonSerializer.Deserialize<TResponse>(cachedResponse, jsonOptions)!;

            // 4 - If the cached response is null
            // we will call the next handler in the pipeline and cache the response
            var response = await next();

            // 5 - use the Reflection to check if the response has a property called "IsSuccess"
            // and if it does, check if it is true
            var isSuccessProperty = typeof(TResponse).GetProperty("IsSuccess");
            if (isSuccessProperty != null)
            {
                var isSuccess = (bool)isSuccessProperty.GetValue(response)!;
                if (!isSuccess)
                    return response;
            }
            // 6 - the time to live for the cache entry is determined by the Expiration property of the request,
            // if it is null, we will use a default value of 15 minutes
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = request.Expiration ?? TimeSpan.FromMinutes(15)
            };

            // 7 - Cache the response in Redis
            await _cache.SetStringAsync(request.CacheKey, JsonSerializer.Serialize(response, jsonOptions), options, cancellationToken);

            return response;

        }
    }
}
