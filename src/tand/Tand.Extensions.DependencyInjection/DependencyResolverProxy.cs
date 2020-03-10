using System;
using Tand.Core;

namespace Tand.Extensions.DependencyInjection
{
    public class DependencyResolverProxy : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyResolverProxy(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ITandTarget<T> TargetOfType<T>(Type type) => _serviceProvider.GetService(type) switch
        {
            ITandTarget<T> target => target,
            _ => throw new ArgumentException()
        };
    }
}