using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Revo.Application.Abstraction.Services;
using Revo.Infrastructure.Database;
using Revo.IntegrationTests.Infrastructre.FakeExternalService;
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
                
                // remove the upload service if it exists
                var uploadServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IUploadService));
                if (uploadServiceDescriptor != null)
                    services.Remove(uploadServiceDescriptor);
                services.AddScoped<IUploadService, FakeUploadService>();

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
            // 1. Start the Docker SQL Server container first so we have a physical database server running.
            await _dbContainer.StartAsync();

            // 2. CREATE A MINI-APP (SANDBOX):
            // We create a temporary, isolated ServiceCollection. 
            // Why? If we use the main application's DI container (Services.CreateScope()), 
            // it will trigger Program.cs, which runs the Data Seeders before the tables even exist!
            var services = new ServiceCollection();

            // 3. Register ONLY what EF Core needs to run migrations (DbContext and Logging).
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_dbContainer.GetConnectionString()));

            services.AddLogging();

            // 4. Build the temporary Dependency Injection provider.
            using var provider = services.BuildServiceProvider();

            // 5. Create a scope to resolve scoped services (like ApplicationDbContext).
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // 6. BUILD THE HOUSE:
            // Execute the migrations on the empty Docker database. 
            // Now the tables are created silently without triggering the main application.
            await context.Database.MigrateAsync();

            // Note: Once this method finishes, the actual Integration Test will start, 
            // triggering the real Program.cs. The Data Seeder will run, find the tables ready, 
            // and seed the initial data successfully!
        }

        // to stop and dispose the container after running tests
        Task IAsyncLifetime.DisposeAsync()
        {
            return _dbContainer.DisposeAsync().AsTask();
        }
    }
}
