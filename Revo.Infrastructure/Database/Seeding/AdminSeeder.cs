using Microsoft.AspNetCore.Identity;
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

            var adminsToSeed = new[]
            {
                new { Email = "admin1@revo.com", FullName = "المدير الأول", Password = "Password@123" },
                new { Email = "admin2@revo.com", FullName = "المدير الثاني", Password = "Password@123" },
                new { Email = "admin3@revo.com", FullName = "المدير الثالث", Password = "Password@123" }
            };

            foreach (var adminData in adminsToSeed)
            {
                var existingUser = await userManager.FindByEmailAsync(adminData.Email);

                if (existingUser == null)
                {
                    var newAdminId = Guid.NewGuid();

                    var identityResult = await identityService.RegisterAdminAsync(
                        newAdminId,
                        adminData.Email,
                        adminData.Password,
                        adminData.FullName,
                        CancellationToken.None);

                    if (identityResult.IsSuccess)
                    {
                        var domainAdmin = new Admin
                        {
                            Id = newAdminId,
                            FullName = adminData.FullName,
                            IsDeleted = false
                        };

                        await adminRepo.AddAsync(domainAdmin, CancellationToken.None);

                    }
                }
            }
        }
    }
}