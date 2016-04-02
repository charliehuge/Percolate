using System.Collections.Generic;
using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class TreeInfo : ScriptableObject
    {
        public NodeInfo RootNode;
        public List<NodeInfo> AllNodes = new List<NodeInfo>();
    }
}
