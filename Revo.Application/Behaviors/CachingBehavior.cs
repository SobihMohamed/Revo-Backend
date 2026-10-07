using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
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
        where TRequest : IRequest<TResponse>, ICacheableQuery
    {
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
        private readonly IDistributedCache _cache;
        public CachingBehavior(IDistributedCache cache , ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            this._cache = cache;
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // 1 - If the request does not have a cache key or cache group, we will just call the next handler in the pipeline
            if (string.IsNullOrEmpty(request.CacheKey) || string.IsNullOrEmpty(request.CacheGroup))
                return await next();
            // 2 - When we conver Result to JSON, we want to ignore null values and be case insensitive when deserializing
            // to save space of Redis and avoid issues with case sensitivity when deserializing
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            string? fullCacheKey = null;
            try
            {
                // =========================================================================
                // STEP 3: Get or Create the "Version Number" for this Group
                // =========================================================================

                // 3.1 - Prepare the name of the key that holds ONLY the version number.
                // Ordering: "Version_" + CacheGroup
                // Example: If request.CacheGroup is "Categories", then groupVersionKey = "Version_Categories"
                var groupVersionKey = $"Version_{request.CacheGroup}";

                // 3.2 - Ask Redis: "What is the current version number stored inside 'Version_Categories'?"
                // Example: If it exists in Redis, version = "a1b2c3" (or null if it doesn't exist yet)
                var version = await _cache.GetStringAsync(groupVersionKey, cancellationToken);

                // 3.3 - If Redis returned null (because it's the first time, or Admin deleted it after an Update/Create):
                if (string.IsNullOrEmpty(version))
                {
                    // Generate a brand new random version number (e.g., version = "x9y8z7")
                    version = Guid.NewGuid().ToString("N");

                    // Save this new version inside Redis under the key "Version_Categories"
                    // Now in Redis: Key = "Version_Categories" ---> Value = "x9y8z7"
                    await _cache.SetStringAsync(groupVersionKey, version, cancellationToken);
                }

                // =========================================================================
                // STEP 4: Build the Final Key for the Actual Data
                // =========================================================================

                // 4.1 - Combine the 3 parts together to make the final key for the page data.
                // Ordering: [1. CacheGroup] _ [2. version] _ [3. CacheKey]
                // Example: "Categories" + "_" + "x9y8z7" + "_" + "Page_1_Size_10"
                // Result: fullCacheKey = "Categories_x9y8z7_Page_1_Size_10"
                fullCacheKey = $"{request.CacheGroup}_{version}_{request.CacheKey}";

                // =========================================================================
                // STEP 5: Get the Actual Data from Redis using the Final Key
                // =========================================================================

                // 5.1 - Ask Redis: "Do you have the JSON data for 'Categories_x9y8z7_Page_1_Size_10'?"
                var cachedResponse = await _cache.GetStringAsync(fullCacheKey, cancellationToken);

                // 5.2 - If we found the JSON data in Redis, convert it back to C# Object (Result<T>) and return it immediately!
                if (!string.IsNullOrEmpty(cachedResponse))
                    return JsonSerializer.Deserialize<TResponse>(cachedResponse, jsonOptions)!;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis is down or unreachable during Read. Falling back to SQL Database.");
            }
            // 6 - If the cached response is null
            // we will call the next handler in the pipeline and cache the response
            var response = await next();

            // 7 - use the Reflection to check if the response has a property called "IsSuccess"
            // and if it does, check if it is true
            var isSuccessProperty = typeof(TResponse).GetProperty("IsSuccess");
            if (isSuccessProperty != null)
            {
                var isSuccess = (bool)isSuccessProperty.GetValue(response)!;
                if (!isSuccess)
                    return response;
            }
            
            if (fullCacheKey != null)
            {
                try
                {
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = request.Expiration ?? TimeSpan.FromMinutes(15)
                    };

                    await _cache.SetStringAsync(fullCacheKey, JsonSerializer.Serialize(response, jsonOptions), options, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to write to Redis cache.");
                }
            }
            return response;

        }
    }
}
