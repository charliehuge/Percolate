using System;
using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class Blackboard : MonoBehaviour
    {
        public enum ParamType
        {
            Int,
            Float,
            String   
        }

        [Serializable]
        public class Param
        {
            public string Name;
            public ParamType Type;
            public int IntValue;
            public float FloatValue;
            public string StringValue;
        }

        [SerializeField] private Param[] _params;

        public Param GetParam(string pName)
        {
            foreach (var param in _params)
            {
                if (param.Name == pName)
                {
                    return param;
                }
            }

            return null;
        }

        public void SetParam(string pName, int value)
        {
            var p = GetParam(pName);

            if (p != null && p.Type == ParamType.Int)
            {
                p.IntValue = value;
            }
        }

        public void SetParam(string pName, float value)
        {
            var p = GetParam(pName);

            if (p != null && p.Type == ParamType.Float)
            {
                p.FloatValue = value;
            }
        }

        public void SetParam(string pName, string value)
        {
            var p = GetParam(pName);

            if (p != null && p.Type == ParamType.String)
            {
                p.StringValue = value;
            }
        }
    }
}
