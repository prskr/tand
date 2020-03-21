using System;
using System.Diagnostics;
using Tand.Core.Api;
using Tand.Core.Models;

namespace Tand.Core.Benchmarks
{
    public class ExecutionTimeTand<T> : ITandTarget<T>
    {
        private readonly Stopwatch _stopwatch;
        private readonly Action<string>? _resultHandler;

        public ExecutionTimeTand() : this(null)
        {
            
        }

        public ExecutionTimeTand(Action<string>? resultHandler)
        {
            _stopwatch = new Stopwatch();
            _resultHandler = resultHandler;
        }

        public void OnEnterMethod(CallEnterContext<T> enterContext)
        {
            _stopwatch.Start();
        }

        public void OnLeaveMethod(CallLeaveContext<T> leaveContext)
        {
            _stopwatch.Stop();
            _resultHandler?.Invoke($"Total execution time: {_stopwatch.Elapsed}");
        }
    }
}