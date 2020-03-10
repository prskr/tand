using System.Collections.Generic;
using System.Linq;
using Tand.Core.Models;

namespace Tand.Core.Models
{
    public readonly struct CallEnterContext<T>
    {
        private readonly IDictionary<string, object> _methodArgs;

        public CallEnterContext(T instance, IDictionary<string, object> args)
        {
            Instance = instance;
            _methodArgs = args;
        }

        public T Instance { get; }

        public object? this[string argName] => _methodArgs.ContainsKey(argName) ? _methodArgs[argName] : null;

        public IEnumerable<MethodArgument> Arguments => _methodArgs.Select(kv => new MethodArgument(kv.Key, kv.Value));
    }
}