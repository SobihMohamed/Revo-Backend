using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Revo.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Text;
using Testcontainers.MsSql;

namespace Revo.IntegrationTests.Infrastructre
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        // define container for SQL Server Docker
        private readonly MsSqlContainer _dbContainer;

        public CustomWebApplicationFactory()
        {
            _dbContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("Revo_Test_Password_123!")
                .Build();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // get the main db connection from program and remove it
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);
                // add a new db context with the connection string from the container
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(_dbContainer.GetConnectionString());
                });
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.DefaultScheme;
                    options.DefaultChallengeScheme = TestAuthHandler.DefaultScheme;
                }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.DefaultScheme, options => { });
            });
        }
        // to start the container before running tests
        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync(); 
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
        }

        // to stop and dispose the container after running tests
        Task IAsyncLifetime.DisposeAsync()
        {
            return _dbContainer.DisposeAsync().AsTask();
        }
    }
}
