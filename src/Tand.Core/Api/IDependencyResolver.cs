using System;

namespace Tand.Core.Api
{
    public interface IDependencyResolver
    {
        ITandTarget<T>? TargetOfType<T>(Type type);
    }
}