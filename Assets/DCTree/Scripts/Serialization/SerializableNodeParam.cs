using System;

namespace DerelictComputer.DCTree
{
    [Serializable]
    public class SerializableNodeParam
    {
        public string FieldName;
        public int IntValue;
        public float FloatValue;
        public string StringValue;
        [NonSerialized] public Type Type;
        [NonSerialized] public object Min;
        [NonSerialized] public object Max;
    }
}
