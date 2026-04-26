using UnityEngine;
using System;

namespace Common.Unity.Editor
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class LayerFieldAttribute : PropertyAttribute
    {
    }
}
