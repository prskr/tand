using Tand.Core.Models;

namespace Tand.Core.Api
{
    public interface ITandTarget<T>
    {
        void OnEnterMethod(CallEnterContext<T> enterContext);

        void OnLeaveMethod(CallLeaveContext<T> leaveContext);
    }
}