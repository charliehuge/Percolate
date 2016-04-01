using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DerelictComputer.DCTree
{
    /// <summary>
    /// Attribute for tagging params in nodes for serialization
    /// Empty because we can infer everything else from the type and other attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class NodeParamAttribute : Attribute
    { 
    }

    [Serializable]
    public abstract class NodeParamInfo
    {
        public Type Type;
        public string Name;

        public override string ToString()
        {
            return string.Format("[{0}] {1}", Type.FullName, Name);
        }
    }

    [Serializable]
    public class NodeParamInfoUint : NodeParamInfo
    {
        [NonSerialized] public bool HasRange;
        [NonSerialized] public uint Min;
        [NonSerialized] public uint Max;
        public uint Value;

        public override string ToString()
        {
            return string.Format("{0} - HasRange: {1} Min: {2} Max: {3} Value: {4}", base.ToString(), HasRange, Min, Max,
                Value);
        }
    }

    [Serializable]
    public class NodeParamInfoInt : NodeParamInfo
    {
        [NonSerialized] public bool HasRange;
        [NonSerialized] public int Min;
        [NonSerialized] public int Max;
        public int Value;

        public override string ToString()
        {
            return string.Format("{0} - HasRange: {1} Min: {2} Max: {3} Value: {4}", base.ToString(), HasRange, Min, Max,
                Value);
        }
    }

    [Serializable]
    public class NodeParamInfoFloat : NodeParamInfo
    {
        [NonSerialized] public bool HasRange;
        [NonSerialized] public float Min;
        [NonSerialized] public float Max;
        public float Value;

        public override string ToString()
        {
            return string.Format("{0} - HasRange: {1} Min: {2} Max: {3} Value: {4}", base.ToString(), HasRange, Min, Max,
                Value);
        }
    }

    [Serializable]
    public class NodeParamInfoDouble : NodeParamInfo
    {
        [NonSerialized] public bool HasRange;
        [NonSerialized] public double Min;
        [NonSerialized] public double Max;
        public double Value;

        public override string ToString()
        {
            return string.Format("{0} - HasRange: {1} Min: {2} Max: {3} Value: {4}", base.ToString(), HasRange, Min, Max,
                Value);
        }
    }

    [Serializable]
    public class NodeParamInfoString : NodeParamInfo
    {
        public string Value;

        public override string ToString()
        {
            return string.Format("{0} - Value: {1}", base.ToString(), Value);
        }
    }

    [Serializable]
    public class NodeParamInfoNode : NodeParamInfo
    {
        public NodeInfo Node;

        public override string ToString()
        {
            return string.Format("{0} - Node: {1}", base.ToString(), Node);
        }
    }

    [Serializable]
    public class NodeParamInfoNodeArray : NodeParamInfo
    {
        public readonly List<NodeInfo> Nodes = new List<NodeInfo>();

        public override string ToString()
        {
            var s = base.ToString() + " - Nodes {";

            s = Nodes.Aggregate(s, (current, nodeInfo) => current + nodeInfo);

            s += "}";
            return s;
        }
    }

    [Serializable]
    public class NodeInfo
    {
        public readonly Type NodeType;
        public readonly List<NodeParamInfo> NodeParams;

        public NodeInfo(Type nodeType)
        {
            NodeType = nodeType;
            NodeParams = new List<NodeParamInfo>();

            foreach (var fieldInfo in nodeType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                Debug.Log("asdf" + fieldInfo.Name);
                var attributes = fieldInfo.GetCustomAttributes(true);

                foreach (var attribute in attributes)
                {
                    if (!(attribute is NodeParamAttribute)) continue;

                    if (fieldInfo.FieldType == typeof(uint))
                    {
                        var npi = new NodeParamInfoUint();
                        npi.Type = fieldInfo.FieldType;
                        npi.Name = fieldInfo.Name;

                        foreach (var a2 in attributes)
                        {
                            var ra = a2 as RangeAttribute;
                            if (ra != null)
                            {
                                npi.HasRange = true;
                                npi.Min = (uint) ra.min;
                                npi.Max = (uint) ra.max;
                                break;
                            }
                        }

                        NodeParams.Add(npi);
                        break;
                    }
                    if (fieldInfo.FieldType == typeof(int))
                    {
                        var npi = new NodeParamInfoInt();
                        npi.Type = fieldInfo.FieldType;
                        npi.Name = fieldInfo.Name;

                        foreach (var a2 in attributes)
                        {
                            var ra = a2 as RangeAttribute;
                            if (ra != null)
                            {
                                npi.HasRange = true;
                                npi.Min = (int)ra.min;
                                npi.Max = (int)ra.max;
                                break;
                            }
                        }

                        NodeParams.Add(npi);
                        break;
                    }
                    if (fieldInfo.FieldType == typeof(float))
                    {
                        var npi = new NodeParamInfoFloat();
                        npi.Type = fieldInfo.FieldType;
                        npi.Name = fieldInfo.Name;

                        foreach (var a2 in attributes)
                        {
                            var ra = a2 as RangeAttribute;
                            if (ra != null)
                            {
                                npi.HasRange = true;
                                npi.Min = ra.min;
                                npi.Max = ra.max;
                                break;
                            }
                        }

                        NodeParams.Add(npi);
                        break;
                    }
                    if (fieldInfo.FieldType == typeof(double))
                    {
                        var npi = new NodeParamInfoDouble();
                        npi.Type = fieldInfo.FieldType;
                        npi.Name = fieldInfo.Name;

                        foreach (var a2 in attributes)
                        {
                            var ra = a2 as RangeAttribute;
                            if (ra != null)
                            {
                                npi.HasRange = true;
                                npi.Min = ra.min;
                                npi.Max = ra.max;
                                break;
                            }
                        }

                        NodeParams.Add(npi);
                        break;
                    }
                    if (fieldInfo.FieldType == typeof(string))
                    {
                        var npi = new NodeParamInfoString();
                        npi.Type = fieldInfo.FieldType;
                        npi.Name = fieldInfo.Name;
                        NodeParams.Add(npi);
                        break;
                    }
                    if (fieldInfo.FieldType == typeof (Node))
                    {
                        var npi = new NodeParamInfoNode();
                        npi.Type = fieldInfo.FieldType;
                        npi.Name = fieldInfo.Name;
                        NodeParams.Add(npi);
                        break;
                    }
                    if (fieldInfo.FieldType == typeof (Node[]))
                    {
                        var npi = new NodeParamInfoNodeArray();
                        npi.Type = fieldInfo.FieldType;
                        npi.Name = fieldInfo.Name;
                        NodeParams.Add(npi);
                        break;
                    }

                    Debug.LogWarning("Unsupported type tagged in " + nodeType.Name + " (" + fieldInfo.FieldType.Name + ")");
                }
            }
        }

        public override string ToString()
        {
            return NodeParams.Aggregate(NodeType.FullName, (current, nodeParamInfo) => current + (" " + nodeParamInfo));
        }
    }
}