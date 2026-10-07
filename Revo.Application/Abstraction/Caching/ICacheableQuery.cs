using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Abstraction.Caching
{
    public interface ICacheableQuery
    {
        // group name to identify the cache entry for this query to use it in virsioning and invalidation of the cache entry
        string CacheGroup { get; } 
        // unique key to identify the cache entry for this query
        string CacheKey { get; }

        // time to live (TTL) for the cache entry, after which it will expire and be removed from the cache
        TimeSpan? Expiration { get; }


    }
}
