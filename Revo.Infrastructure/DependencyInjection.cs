using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Revo.Application.Abstraction.Services;
using Revo.Application.Contracts;
using Revo.Application.Contracts.Auth;
using Revo.Application.Contracts.Identity;
using Revo.Application.Contracts.Notifications;
using Revo.Application.Contracts.Repositories;
using Revo.Infrastructure.Database;
using Revo.Infrastructure.Identity;
using Revo.Infrastructure.Implementations;
using Revo.Infrastructure.Implementations.Notifications;
using Revo.Infrastructure.Implementations.Notifications.Strategies;
using Revo.Infrastructure.Repos;
using Revo.Infrastructure.ServicesImplementation;
using Revo.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Revo.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Database Configuration
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // 2. Identity Configuration (Core Only - No Web Dependencies)
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
            services.Configure<SiteSettings>(configuration.GetSection("SiteSettings"));
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.Configure<TwilioSettings>(configuration.GetSection("Twilio"));

            // 3. Repositories & Unit of Work Registration
            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUploadService, CloudinaryService>();
            services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
            services.AddScoped<INotificationStrategy, PushNotificationStrategy>();
            services.AddScoped<INotificationStrategy, TwilioWhatsAppNotificationStrategy>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IIdentityService, IdentityService>();
            return services;
        }
    }
}