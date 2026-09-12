using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Revo.Application.Abstraction;
using System.Reflection;

namespace Revo.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            // get the assembly where the validators and MediatR handlers are defined
            var assembly = Assembly.GetExecutingAssembly();

            // 1. Register all validators from the assembly
            services.AddValidatorsFromAssembly(assembly);

            // 2. Register MediatR and add the ValidationBehavior pipeline
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);

                // add the ValidationBehavior to the MediatR pipeline
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            return services;
        }
    }
}