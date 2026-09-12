using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Domain.Common
{
    public interface IEntity<TId>
    {
        public TId Id { get; set; }
    }
}
