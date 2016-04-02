using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

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

    [AttributeUsage(AttributeTargets.Field)]
    public class NodeChildAttribute : Attribute
    {
    }

    [Serializable]
    public abstract class NodeParamInfo
    {
        public Type Type;
        public string Name;

        protected NodeParamInfo(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", Type.FullName, Name);
        }

        public abstract void DrawEditor(Rect windowRect);
    }

    [Serializable]
    public class NodeParamInfoUint : NodeParamInfo
    {
        [NonSerialized] public bool HasRange;
        [NonSerialized] public int Min;
        [NonSerialized] public int Max;
        public uint Value;

        public NodeParamInfoUint(Type type, string name) : base(type, name)
        {
            HasRange = false;
        }

        public NodeParamInfoUint(Type type, string name, int min, int max) : base(type, name)
        {
            HasRange = true;
            Min = min < 0 ? 0 : min;
            Max = max < 0 ? 0 : max;
        }

        public override string ToString()
        {
            return string.Format("{0} - HasRange: {1} Min: {2} Max: {3} Value: {4}", base.ToString(), HasRange, Min, Max,
                Value);
        }

        public override void DrawEditor(Rect windowRect)
        {
            if (HasRange)
            {
                Value = (uint) EditorGUILayout.IntSlider(Name, (int) Value, Min, Max);
            }
            else
            {
                Value = (uint) EditorGUILayout.IntField(Name, (int) Value);
            }
        }
    }

    [Serializable]
    public class NodeParamInfoInt : NodeParamInfo
    {
        [NonSerialized] public bool HasRange;
        [NonSerialized] public int Min;
        [NonSerialized] public int Max;
        public int Value;

        public NodeParamInfoInt(Type type, string name) : base(type, name)
        {
            HasRange = false;
        }

        public NodeParamInfoInt(Type type, string name, int min, int max) : base(type, name)
        {
            HasRange = true;
            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return string.Format("{0} - HasRange: {1} Min: {2} Max: {3} Value: {4}", base.ToString(), HasRange, Min, Max,
                Value);
        }

        public override void DrawEditor(Rect windowRect)
        {
            if (HasRange)
            {
                Value = EditorGUILayout.IntSlider(Name, Value, Min, Max);
            }
            else
            {
                Value = EditorGUILayout.IntField(Name, Value);
            }
        }
    }

    [Serializable]
    public class NodeParamInfoFloat : NodeParamInfo
    {
        [NonSerialized] public bool HasRange;
        [NonSerialized] public float Min;
        [NonSerialized] public float Max;
        public float Value;

        public NodeParamInfoFloat(Type type, string name) : base(type, name)
        {
            HasRange = false;
        }

        public NodeParamInfoFloat(Type type, string name, float min, float max) : base(type, name)
        {
            HasRange = true;
            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return string.Format("{0} - HasRange: {1} Min: {2} Max: {3} Value: {4}", base.ToString(), HasRange, Min, Max,
                Value);
        }

        public override void DrawEditor(Rect windowRect)
        {
            if (HasRange)
            {
                Value = EditorGUILayout.Slider(Name, Value, Min, Max);
            }
            else
            {
                Value = EditorGUILayout.FloatField(Name, Value);
            }
        }
    }

    [Serializable]
    public class NodeParamInfoDouble : NodeParamInfo
    {
        [NonSerialized] public bool HasRange;
        [NonSerialized] public float Min;
        [NonSerialized] public float Max;
        public double Value;

        public NodeParamInfoDouble(Type type, string name) : base(type, name)
        {
            HasRange = false;
        }

        public NodeParamInfoDouble(Type type, string name, float min, float max) : base(type, name)
        {
            HasRange = true;
            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return string.Format("{0} - HasRange: {1} Min: {2} Max: {3} Value: {4}", base.ToString(), HasRange, Min, Max,
                Value);
        }

        public override void DrawEditor(Rect windowRect)
        {
            if (HasRange)
            {
                Value = EditorGUILayout.Slider(Name, (float)Value, Min, Max);
            }
            else
            {
                Value = EditorGUILayout.FloatField(Name, (float)Value);
            }
        }
    }

    [Serializable]
    public class NodeParamInfoString : NodeParamInfo
    {
        public string Value;

        public NodeParamInfoString(Type type, string name) : base(type, name)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} - Value: {1}", base.ToString(), Value);
        }

        public override void DrawEditor(Rect windowRect)
        {
            Value = EditorGUILayout.TextField(Name, Value);
        }
    }

    [Serializable]
    public abstract class NodeChildInfo
    {
        public readonly string Name;

        protected NodeChildInfo(string name)
        {
            Name = name;
        }

        public abstract bool DrawEditor(Rect windowRect);

        public abstract void AddChild(NodeInfo child);

        public abstract NodeInfo[] GetChildren();
    }

    [Serializable]
    public class NodeChildInfoSingle : NodeChildInfo
    {
        public NodeInfo Child;

        public NodeChildInfoSingle(string name) : base(name)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", base.ToString(), Child);
        }

        public override bool DrawEditor(Rect windowRect)
        {
            var connectorRect = new Rect(windowRect.width / 2, windowRect.height - 12, 10, 10);
            GUI.Box(connectorRect, "");
            EditorGUIUtility.AddCursorRect(connectorRect, MouseCursor.Link);

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (connectorRect.Contains(Event.current.mousePosition))
                    {
                        Event.current.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }

        public override void AddChild(NodeInfo child)
        {
            Child = child;
        }

        public override NodeInfo[] GetChildren()
        {
            return new[] {Child};
        }
    }

    [Serializable]
    public class NodeChildInfoList : NodeChildInfo
    {
        public readonly List<NodeInfo> Children = new List<NodeInfo>(); 

        public NodeChildInfoList(string name) : base(name)
        {
        }

        public override string ToString()
        {
            var s = base.ToString() + " - {";

            s = Children.Aggregate(s, (current, nodeInfo) => current + nodeInfo);

            s += "}";
            return s;
        }

        public override bool DrawEditor(Rect windowRect)
        {
            throw new NotImplementedException();
        }

        public override void AddChild(NodeInfo child)
        {
            if (Children.Contains(child))
            {
                return;
            }

            Children.Add(child);
        }

        public override NodeInfo[] GetChildren()
        {
            return Children.ToArray();
        }
    }

    [Serializable]
    public class NodeInfo
    {
        public readonly Type NodeType;
        public readonly List<NodeParamInfo> NodeParams;
        public readonly NodeChildInfo ChildParam;
        public Vector2 EditorPosition;

        public NodeInfo(Type nodeType)
        {
            NodeType = nodeType;
            NodeParams = new List<NodeParamInfo>();

            foreach (var fieldInfo in nodeType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                var attributes = fieldInfo.GetCustomAttributes(true);

                foreach (var attribute in attributes)
                {
                    var npa = attribute as NodeParamAttribute;
                    var nca = attribute as NodeChildAttribute;

                    if (npa != null)
                    {
                        if (fieldInfo.FieldType == typeof(uint))
                        {
                            if (npa.Min != null && npa.Max != null)
                            {
                                NodeParams.Add(new NodeParamInfoUint(fieldInfo.FieldType, fieldInfo.Name, (int)npa.Min, (int)npa.Max));
                            }
                            else
                            {
                                NodeParams.Add(new NodeParamInfoUint(fieldInfo.FieldType, fieldInfo.Name));
                            }

                            break;
                        }
                        if (fieldInfo.FieldType == typeof(int))
                        {
                            if (npa.Min != null && npa.Max != null)
                            {
                                NodeParams.Add(new NodeParamInfoInt(fieldInfo.FieldType, fieldInfo.Name, (int)npa.Min, (int)npa.Max));
                            }
                            else
                            {
                                NodeParams.Add(new NodeParamInfoInt(fieldInfo.FieldType, fieldInfo.Name));
                            }

                            break;
                        }
                        if (fieldInfo.FieldType == typeof(float))
                        {
                            if (npa.Min != null && npa.Max != null)
                            {
                                NodeParams.Add(new NodeParamInfoFloat(fieldInfo.FieldType, fieldInfo.Name, (float)npa.Min, (float)npa.Max));
                            }
                            else
                            {
                                NodeParams.Add(new NodeParamInfoFloat(fieldInfo.FieldType, fieldInfo.Name));
                            }

                            break;
                        }
                        if (fieldInfo.FieldType == typeof(double))
                        {
                            if (npa.Min != null && npa.Max != null)
                            {
                                NodeParams.Add(new NodeParamInfoDouble(fieldInfo.FieldType, fieldInfo.Name, (float)npa.Min, (float)npa.Max));
                            }
                            else
                            {
                                NodeParams.Add(new NodeParamInfoDouble(fieldInfo.FieldType, fieldInfo.Name));
                            }

                            break;
                        }
                        if (fieldInfo.FieldType == typeof(string))
                        {
                            NodeParams.Add(new NodeParamInfoString(fieldInfo.FieldType, fieldInfo.Name));
                            break;
                        }
                    }
                    else if (nca != null)
                    {
                        if (fieldInfo.FieldType == typeof(Node))
                        {
                            ChildParam = new NodeChildInfoSingle(fieldInfo.Name);
                            break;
                        }
                        if (fieldInfo.FieldType == typeof(Node[]))
                        {
                            ChildParam = new NodeChildInfoList(fieldInfo.Name);
                            break;
                        }
                    }

                    Debug.LogWarning("Unsupported type tagged in " + nodeType.Name + " (" + fieldInfo.FieldType.Name + ")");
                }
            }
        }

        public override string ToString()
        {
            return NodeParams.Aggregate(NodeType.FullName, (current, nodeParamInfo) => current + (" " + nodeParamInfo));
        }

        public bool DrawEditor(Rect windowRect)
        {
            EditorGUIUtility.labelWidth = 60;
            foreach (var nodeParamInfo in NodeParams)
            {
                nodeParamInfo.DrawEditor(windowRect);
            }
            EditorGUIUtility.labelWidth = 0;

            return ChildParam != null && ChildParam.DrawEditor(windowRect);
        }

        public void AddChild(NodeInfo child)
        {
            if (ChildParam == null)
            {
                Debug.LogWarning("Tried to connect something that doesn't have children");
                return;
            }

            ChildParam.AddChild(child);
        }
    }
}