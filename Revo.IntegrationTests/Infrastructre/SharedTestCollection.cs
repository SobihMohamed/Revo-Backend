using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.IntegrationTests.Infrastructre
{
    [CollectionDefinition("SharedTestCollection")]
    public class SharedTestCollection : ICollectionFixture<CustomWebApplicationFactory>
    {
    }
}
