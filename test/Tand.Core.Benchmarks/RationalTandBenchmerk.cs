using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Tand.Core.Api;

namespace Tand.Core.Benchmarks
{
    public class RationalTandBenchmerk
    {

        private readonly SimpleDependencyResolver _dependencyResolver;
        
        public RationalTandBenchmerk()
        {
            _dependencyResolver = new SimpleDependencyResolver();
            _dependencyResolver.Register<ExecutionTimeTand<IRationalsProvider>, IRationalsProvider>(() => new ExecutionTimeTand<IRationalsProvider>());
        }

        [Benchmark]
        public void GenerateWithoutTand()
        {
            var rationalProvider = new RationalsProvider();
            // just to mimic behavior
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = rationalProvider.Generate(Numbers());
            stopWatch.Stop();
        }
        
        [Benchmark]
        public void GenerateWithTand()
        {
            
            var tand = new Tand(_dependencyResolver);
            var rationalProvider = tand.DecorateWithTand<IRationalsProvider, RationalsProvider>(new RationalsProvider());
            var result = rationalProvider.Generate(Numbers());
        }
        
        private static IEnumerable<(int, int)> Numbers() => Enumerable
            .Range(1, 50_000)
            .Zip(Enumerable.Range(25_000, 50_000), (x, y) => (x, y))
            .ToList();
    }

    internal class SimpleDependencyResolver : IDependencyResolver
    {
        private readonly IDictionary<Type, Func<object>> _registeredProducers;

        public SimpleDependencyResolver()
        {
            _registeredProducers = new Dictionary<Type, Func<object>>();
        }

        internal void Register<T, TS>(Func<T> producer) where T : class, ITandTarget<TS>
        {
            _registeredProducers[producer.Method.ReturnType] = producer;
        }

        public ITandTarget<T>? TargetOfType<T>(Type type)
        {
            if (_registeredProducers.ContainsKey(type))
            {
                return _registeredProducers[type]() as ITandTarget<T>;
            }

            return null;
        }
    }
}