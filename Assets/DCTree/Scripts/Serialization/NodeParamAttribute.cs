using System;

namespace DerelictComputer.DCTree
{
    /// <summary>
    /// Attribute for tagging params in nodes for serialization
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class NodeParamAttribute : Attribute
    {
        public readonly object Min;
        public readonly object Max;

        public NodeParamAttribute()
        {
            Min = null;
            Max = null;
        }

        public NodeParamAttribute(object min, object max)
        {
            Min = min;
            Max = max;
        }
    }
}
