using Tand.Core.Models;

namespace Tand.Core
{
    public interface ITandTarget<T>
    {
        void OnEnterMethod(CallEnterContext<T> enterContext);

        void OnLeaveMethod(CallLeaveContext<T> leaveContext);
    }
}