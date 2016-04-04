using System;
using UnityEngine;

namespace DerelictComputer.DCTree
{

    [Serializable]
    public class SerializableNode
    {
        public string AssemblyQualifiedTypeName;
        public Vector2 EditorPosition;
        public SerializableNodeParam[] Params;
        public int ChildCount;
        public int FirstChildIndex;
    }
}
