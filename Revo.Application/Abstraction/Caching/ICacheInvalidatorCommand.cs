using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Abstraction.Caching
{
    public interface ICacheInvalidatorCommand
    {
        // List of cache keys that should be cleared when this command is executed
        string[] CacheKeysToClear { get; }
    }
}
