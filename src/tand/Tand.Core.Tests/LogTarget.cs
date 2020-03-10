using System;
using Tand.Core.Models;

namespace Tand.Core.Tests
{
    public class LogTarget<T> : ITandTarget<T>
    {
        private readonly Action<string> _logHandle;

        public LogTarget(Action<string> logHandle)
        {
            _logHandle = logHandle;
        }

        public void OnEnterMethod(CallEnterContext<T> enterContext)
        {
            _logHandle(enterContext.Instance.ToString());
            foreach (var (name, val) in enterContext.Arguments)
            {
                _logHandle($"name: {name}, value: {val}");
            }
        }

        public void OnLeaveMethod(CallLeaveContext<T> callLeaveContext)
        {
            _logHandle(callLeaveContext.Instance.ToString());
            foreach (var (name, val) in callLeaveContext.Arguments)
            {
                _logHandle($"name: {name}, value: {val}");
            }

            _logHandle($"result: {callLeaveContext.CallResult}");
        }
    }
}