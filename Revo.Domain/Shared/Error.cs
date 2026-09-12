using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Domain.Shared
{
    public record Error(string Code, string Message)
    {
        public static readonly Error None = new Error(string.Empty, string.Empty);
        public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");// Represents a null value error.
    }
}
