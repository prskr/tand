using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tand.Core.Models;

namespace Tand.Core
{
    public class TandProxy<T> : DispatchProxy
    {
        private readonly IDictionary<int, Type[]> _targetCache;

        public TandProxy()
        {
            _targetCache = new ConcurrentDictionary<int, Type[]>();
        }

        public Type ImplementationType { get; set; }
        public T Decorated { private get; set; }

        public IDependencyResolver DependencyResolver { get; set; }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var mappedMethodArgs = CollectArgs(targetMethod, args);
            var enterContext = new CallEnterContext<T>(Decorated, mappedMethodArgs);
            var targets = TargetTypesForMethod(targetMethod);
            foreach (var target in targets)
            {
                target.OnEnterMethod(enterContext);
            }
            
            var result = targetMethod.Invoke(Decorated, args);
            
            var leaveContext = new CallLeaveContext<T>(Decorated, mappedMethodArgs, result);
            
            foreach (var target in targets)
            {
                target.OnLeaveMethod(leaveContext);
            }

            return result;
        }

        private static IDictionary<string, object> CollectArgs(MethodBase methodInfo, IReadOnlyList<object> argValues)
        {
            var result = new Dictionary<string, object>();
            var parameters = methodInfo.GetParameters();
            for (var i = 0; i < parameters.Length && i < argValues.Count; i++)
            {
                result.Add(parameters[i].Name, argValues[i]);
            }

            return result;
        }

        private ICollection<ITandTarget<T>> TargetTypesForMethod(MethodInfo methodInfo)
        {
            var hash = methodInfo.GetHashCode();
            if (!_targetCache.ContainsKey(hash))
            {
                _targetCache[hash] = methodInfo.GetCustomAttributes<TandAttribute>()
                    .Select(attr => attr.TargetType)
                    .ToArray();
            }

            return _targetCache[hash]
                .Select(type => DependencyResolver.TargetOfType<T>(type))
                .ToList();
        }
    }
}