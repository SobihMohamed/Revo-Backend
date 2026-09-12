using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Domain.Common
{
    public class BaseEntity<TId> : IEntity<TId>
    {
        public TId Id { get; set; } = default!;
    }
}
