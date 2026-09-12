using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Domain.Shared
{
    public record ValidationError(Error[] ValidationErrors) 
        : Error("Error.Validation", "The specified result value is invalid.")
    {
    }
}
