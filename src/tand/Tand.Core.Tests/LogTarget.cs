using System;
using Tand.Core.Models;

namespace Tand.Core.Tests
{
    public class LogTarget<T> : ITandTarget<T>
    {
        private readonly Action<T> _instanceHandle;

        public LogTarget(Action<T> instanceHandle)
        {
            _instanceHandle = instanceHandle;
        }

        public void OnEnterMethod(CallEnterContext<T> enterContext)
        {
            _instanceHandle(enterContext.Instance);
        }

        public void OnLeaveMethod(CallLeaveContext<T> callLeaveContext)
        {
            _instanceHandle(callLeaveContext.Instance);
        }
    }
}