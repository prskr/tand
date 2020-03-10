using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tand.Extensions.DependencyInjection
{
    public static class TandServiceExtensions
    {
        public static void AddTand(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton(sp => new Tand.Core.Tand(new DependencyResolverProxy(sp)));
        }

        public static IServiceCollection AddTandTransient<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddTransient<TImplementation>();
            services.AddTransient(GetTandForType<TService, TImplementation>);
            return services;
        }

        private static TService GetTandForType<TService, TImplementation>(IServiceProvider serviceProvider) where TImplementation : class, TService where TService : class
        {
            var tand = serviceProvider.GetService<Tand.Core.Tand>();
            var instance = serviceProvider.GetService<TImplementation>();
            var proxy = tand.DecorateWithTand<TService, TImplementation>(instance);
            return proxy;
        }
    }
}