using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;
using Revo.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.IntegrationTests.Infrastructre
{
    public class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly IServiceScope _scope;
        protected readonly ApplicationDbContext _context;
        protected readonly ISender _sender;
        private Respawner _respawner;
        public BaseIntegrationTest(CustomWebApplicationFactory factory)
        {
            _scope = factory.Services.CreateScope();

            // get the required service for the testing 
            _sender = _scope.ServiceProvider.GetRequiredService<ISender>();
            _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public async Task InitializeAsync()
        {
            _respawner = await Respawner.CreateAsync(_context.Database.GetDbConnection(), new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                TablesToIgnore = new Respawn.Graph.Table[]
                    {
                     "__EFMigrationsHistory"
                    }
            });
            await _respawner.ResetAsync(_context.Database.GetDbConnection());
        }
        public Task DisposeAsync()
        {
            _scope.Dispose();
            return Task.CompletedTask;
        }
    }
}
