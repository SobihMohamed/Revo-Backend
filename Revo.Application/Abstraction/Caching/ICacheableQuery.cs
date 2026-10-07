using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Abstraction.Caching
{
    public interface ICacheableQuery
    {
        // unique key to identify the cache entry for this query
        string CacheKey { get; }

        // time to live (TTL) for the cache entry, after which it will expire and be removed from the cache
        TimeSpan? Expiration { get; }
    }
}
