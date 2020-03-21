using System;
using System.Collections.Generic;
using System.Linq;
using Tand.Core.Api;

namespace Tand.Core.Tests
{
    internal class ResolverMock : IDependencyResolver
    {

        private readonly IDictionary<Type, object> _targets;


        internal ResolverMock(params object[] targets)
        {
            _targets = targets.ToDictionary(target => target.GetType(), target => target);
        }

        public ITandTarget<T> TargetOfType<T>(Type type)
        {
            ResolvingCounter++;
            return (ITandTarget<T>)_targets[type];
        }

        internal int ResolvingCounter { get; private set; }

    }
}