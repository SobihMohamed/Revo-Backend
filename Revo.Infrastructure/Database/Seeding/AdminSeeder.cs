using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Revo.Application.Contracts.Identity;
using Revo.Application.Contracts.Repositories;
using Revo.Domain.Entities;
using Revo.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure.Database.Seeding
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var adminRepo = scope.ServiceProvider.GetRequiredService<IGenericRepo<Admin>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }

            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();


            foreach (var section in config.GetSection("SeedAdmins").GetChildren())
            {
                var email = section["Email"];
                var fullName = section["FullName"] ?? "Admin";
                var password = section["Password"];

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    continue;

                var existingUser = await userManager.FindByEmailAsync(email);

                if (existingUser == null)
                {
                    var newAdminId = Guid.NewGuid();

                    var identityResult = await identityService.RegisterAdminAsync(
                        newAdminId,
                        email,
                        password,
                        fullName,
                        CancellationToken.None);

                    if (identityResult.IsSuccess)
                    {
                        var domainAdmin = new Admin
                        {
                            Id = newAdminId,
                            FullName = fullName,
                            IsDeleted = false
                        };

                        await adminRepo.AddAsync(domainAdmin, CancellationToken.None);
                    }
                }
            }
        }
    }
}