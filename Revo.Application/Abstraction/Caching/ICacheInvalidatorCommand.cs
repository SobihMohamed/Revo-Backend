using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Application.Abstraction.Caching
{
    public interface ICacheInvalidatorCommand
    {
        // List of cache groups that should be cleared when this command is executed
        string[] CacheGroupsToClear { get; }
    }
}
