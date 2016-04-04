using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DerelictComputer.DCTree
{

    public class EditorNode
    {
        public enum NodeChildType
        {
            None,
            Single,
            Multiple
        }

        public readonly SerializableNode NodeData;

        public readonly List<EditorNode> Children = new List<EditorNode>();

        public EditorNode Parent { get; private set; }

        public Vector2[] ConnectorPositions { get; private set; }

        public NodeChildType ChildType { get; private set; }

        public string DisplayName { get; private set; }

        public EditorNode(Type nodeType)
        {
            ConnectorPositions = new Vector2[0];
            NodeData = CreateDefaultData(nodeType);
        }

        public EditorNode(SerializableNode nodeData)
        {
            ConnectorPositions = new Vector2[0];
            NodeData = CreateDefaultData(Type.GetType(nodeData.AssemblyQualifiedTypeName));
            NodeData.EditorPosition = nodeData.EditorPosition;
            NodeData.ChildCount = nodeData.ChildCount;
            NodeData.FirstChildIndex = nodeData.FirstChildIndex;

            // copy over the params
            for (int i = 0; i < NodeData.Params.Length; i++)
            {
                for (int j = 0; j < nodeData.Params.Length; j++)
                {
                    if (NodeData.Params[i].FieldName == nodeData.Params[i].FieldName)
                    {
                        var o = nodeData.Params[i];
                        var n = NodeData.Params[i];
                        n.FloatValue = o.FloatValue;
                        n.IntValue = o.IntValue;
                        n.StringValue = o.StringValue;
                        break;
                    }
                }
            }
        }

        public bool AddChild(EditorNode child)
        {
            if (ChildType == NodeChildType.None)
            {
                Debug.LogWarning("Tried to connect something that can't have children");
                return false;
            }

            switch (ChildType)
            {
                case NodeChildType.None:
                    return false;
                case NodeChildType.Single:
                    if (ValidateChild(this, child))
                    {
                        if (child.Parent != null)
                        {
                            child.Parent.RemoveChild(child);
                        }

                        if (Children.Count == 0)
                        {
                            Children.Add(child);
                        }
                        else
                        {
                            Children[0] = child;
                        }

                        child.Parent = this;
                        return true;
                    }
                    return false;
                case NodeChildType.Multiple:
                    if (ValidateChild(this, child))
                    {
                        if (child.Parent != null)
                        {
                            child.Parent.RemoveChild(child);
                        }
                        Children.Add(child);
                        child.Parent = this;
                        return true;
                    }
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void RemoveChild(EditorNode child)
        {
            if (Children.Contains(child))
            {
                Children.Remove(child);
            }
        }

        public void RemoveParent()
        {
            if (Parent == null)
            {
                return;
            }

            Parent.RemoveChild(this);
            Parent = null;
        }

        private static bool ValidateChild(EditorNode parent, EditorNode child)
        {
            // detect loop
            if (parent == child)
            {
                return false;
            }

            if (child.ChildType == NodeChildType.None)
            {
                return true;
            }

            foreach (var c in child.Children)
            {
                if (!ValidateChild(parent, c))
                {
                    return false;
                }
            }

            return true;
        }

        public bool DrawEditor(Rect windowRect)
        {
            foreach (var param in NodeData.Params)
            {
                EditorParam.DrawEditor(param);
            }

            switch (ChildType)
            {
                case NodeChildType.Single:
                    return DrawChildEditorSingle(windowRect);
                case NodeChildType.Multiple:
                    return DrawChildEditorMultiple(windowRect);
            }

            return false;
        }

        private bool DrawChildEditorSingle(Rect windowRect)
        {
            var connectorRect = new Rect(windowRect.width / 2, windowRect.height - 12, 10, 10);
            ConnectorPositions = new[]
            {new Vector2(connectorRect.x + connectorRect.width/2, connectorRect.y + connectorRect.height)};
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

        private bool DrawChildEditorMultiple(Rect windowRect)
        {
            const float connectorSize = 10;
            const float connectorPadding = 2;

            ConnectorPositions = new Vector2[Children.Count + 1];
            var connectorRect = new Rect(0, windowRect.height - connectorSize - connectorPadding, connectorSize, connectorSize);

            for (int i = 0; i < Children.Count; i++)
            {
                ConnectorPositions[i] = new Vector2(connectorRect.x + connectorRect.width / 2, connectorRect.y + connectorRect.height);
                GUI.Box(connectorRect, "");
                connectorRect.x += connectorSize + connectorPadding;
            }

            ConnectorPositions[Children.Count] = new Vector2(connectorRect.x + connectorRect.width / 2, connectorRect.y + connectorRect.height);
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

        private SerializableNode CreateDefaultData(Type type)
        {
            if (type.IsAbstract || !type.IsSubclassOf(typeof (Node)))
            {
                Debug.LogError(type.AssemblyQualifiedName + " is not a subclass of Node");
                return null;
            }

            DisplayName = type.Name;

            ChildType = NodeChildType.None;

            var sNode = new SerializableNode
            {
                AssemblyQualifiedTypeName = type.AssemblyQualifiedName,
                ChildCount = 0,
                FirstChildIndex = -1,
                EditorPosition = Vector2.zero
            };

            var nodeParams = new List<SerializableNodeParam>();

            foreach (var fieldInfo in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            { 
                var attributes = fieldInfo.GetCustomAttributes(true);

                foreach (var attribute in attributes)
                {
                    var npa = attribute as NodeParamAttribute;
                    var nca = attribute as NodeChildAttribute;

                    if (npa != null)
                    {
                        if (fieldInfo.FieldType == typeof (int) || fieldInfo.FieldType == typeof (uint))
                        {
                            nodeParams.Add(new SerializableNodeParam
                            {
                                Type = typeof (int),
                                FieldName = fieldInfo.Name,
                                Min = npa.Min,
                                Max = npa.Max
                            });
                            break;
                        }

                        if (fieldInfo.FieldType == typeof (float) || fieldInfo.FieldType == typeof (double))
                        {
                            nodeParams.Add(new SerializableNodeParam
                            {
                                Type = typeof(float),
                                FieldName = fieldInfo.Name,
                                Min = npa.Min,
                                Max = npa.Max
                            });
                            break;
                        }

                        if (fieldInfo.FieldType == typeof(string))
                        {
                            nodeParams.Add(new SerializableNodeParam
                            {
                                Type = typeof(string),
                                FieldName = fieldInfo.Name
                            });
                            break;
                        }

                        Debug.LogWarning("Unsupported param tagged in " + sNode.AssemblyQualifiedTypeName + " (" + fieldInfo.FieldType.Name + ")");
                    }
                    else if (nca != null)
                    {
                        if (fieldInfo.FieldType == typeof(Node))
                        {
                            ChildType = NodeChildType.Single;
                            break;
                        }

                        if (fieldInfo.FieldType == typeof(Node[]))
                        {
                            ChildType = NodeChildType.Multiple;
                            break;
                        }

                        Debug.LogWarning("Unsupported child tagged in " + sNode.AssemblyQualifiedTypeName + " (" + fieldInfo.FieldType.Name + ")");
                    }
                }
            }

            sNode.Params = nodeParams.ToArray();
            return sNode;
        }
    }
}
