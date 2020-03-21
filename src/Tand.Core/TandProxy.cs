using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Tand.Core.Api;
using Tand.Core.Models;

namespace Tand.Core
{
    public class TandProxy<T> : DispatchProxy
    {
        private readonly IDictionary<int, Type[]> _targetCache;
        private readonly Action<Exception>? _exceptionHandler;

        public TandProxy() : this(null)
        {
        }

        public TandProxy(Action<Exception>? exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
            _targetCache = new ConcurrentDictionary<int, Type[]>();
        }

        public Type ImplementationType { get; set; } = default!;
        public T Decorated { private get; set; } = default!;

        public IDependencyResolver DependencyResolver { get; set; } = default!;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var mappedMethodArgs = CollectArgs(targetMethod, args);
            var targets = TargetTypesForMethod(targetMethod);
            
            ProcessOnEntering(targets, new CallEnterContext<T>(Decorated, mappedMethodArgs));
            
            var result = targetMethod.Invoke(Decorated, args);
            
            ProcessOnLeaving(targets, new CallLeaveContext<T>(Decorated, mappedMethodArgs, result));
            return result;
        }

        protected virtual void OnEnterMethod(ITandTarget<T> target, CallEnterContext<T> callEnterContext)
        {
            try
            {
                target.OnEnterMethod(callEnterContext);
            }
            catch (Exception e)
            {
                _exceptionHandler?.Invoke(e);
            }
        }

        protected virtual void OnLeaveMethod(ITandTarget<T> target, CallLeaveContext<T> callLeaveContext)
        {
            try
            {
                target.OnLeaveMethod(callLeaveContext);
            }
            catch (Exception e)
            {
                _exceptionHandler?.Invoke(e);
            }
        }

        private void ProcessOnEntering(IEnumerable<ITandTarget<T>> targets, CallEnterContext<T> callEnterContext)
        {
            foreach (var tandTarget in targets)
            {
                OnEnterMethod(tandTarget, callEnterContext);
            }
        }
        
        private void ProcessOnLeaving(IEnumerable<ITandTarget<T>> targets, CallLeaveContext<T> callLeaveContext)
        {
            foreach (var tandTarget in targets)
            {
                OnLeaveMethod(tandTarget, callLeaveContext);
            }
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
                .Where(target => target != null)
                .ToList()!;
        }
    }
}