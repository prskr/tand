using System;

namespace Tand.Core
{
    public interface IDependencyResolver
    {
        ITandTarget<T> TargetOfType<T>(Type type);
    }
}