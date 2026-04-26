using System;

namespace Common.Reflection
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ExposeFieldAttribute : Attribute
    {
    }
}