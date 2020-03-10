using System;

namespace Tand.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TandAttribute : Attribute
    {

        public TandAttribute(Type targetType)
        {
            TargetType = targetType;
        }

        public Type TargetType { get; }
    }
}