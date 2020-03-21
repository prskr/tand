using System.Collections.Generic;
using System.Linq;

namespace Tand.Core.Models
{
    public readonly struct CallLeaveContext<T>
    {
        private readonly IDictionary<string, object> _methodArgs;

        public CallLeaveContext(
            T instance,
            IDictionary<string, object> args,
            object callResult
        )
        {
            Instance = instance;
            _methodArgs = args;
            CallResult = callResult;
        }

        public T Instance { get; }

        public object CallResult { get; }

        public object? this[string argName] => _methodArgs.ContainsKey(argName) ? _methodArgs[argName] : null;

        public IEnumerable<MethodArgument> Arguments => _methodArgs.Select(kv => new MethodArgument(kv.Key, kv.Value));
    }
}