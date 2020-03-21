using System.Reflection;
using Tand.Core.Api;

namespace Tand.Core
{
    public class Tand
    {
        private readonly IDependencyResolver _dependencyResolver;

        public Tand(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public TService DecorateWithTand<TService, TImplementation>(TImplementation toBeDecorated)
            where TService : class
            where TImplementation : class, TService
        {
            var proxy = DispatchProxy.Create<TService, TandProxy<TService>>();
            InitProxy<TService, TImplementation>(proxy, toBeDecorated);
            return proxy;
        }

        private void InitProxy<TService, TImplementation>(object proxyObj, TService toBeDecorated)
        {
            var proxy = proxyObj switch
            {
                TandProxy<TService> tp => tp,
                _ => null
            };

            if (proxy == null) return;

            proxy.Decorated = toBeDecorated;
            proxy.ImplementationType = typeof(TImplementation);
            proxy.DependencyResolver = _dependencyResolver;
        }
    }
}