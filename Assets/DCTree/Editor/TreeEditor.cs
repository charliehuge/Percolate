using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class TreeEditor : EditorWindow
    {
        private readonly List<NodeWindow> _nodeWindows = new List<NodeWindow>();
        private NodeWindow _rootNode;
        private NodeWindow _connectingWindow;
        private bool _draggingRoot;

        [MenuItem("Window/DCTree Editor")]
        private static void Init()
        {
            GetWindow<TreeEditor>().Show();
        }

        private void OrderNodeList(List<SerializableNode> collectorList, EditorNode node)
        {
            node.NodeData.ChildCount = node.Children.Count;
            node.NodeData.FirstChildIndex = collectorList.Count + 1;
            collectorList.Add(node.NodeData);

            foreach (var child in node.Children)
            {
                OrderNodeList(collectorList, child);
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("New"))
            {
                _nodeWindows.Clear();
            }

            if (GUILayout.Button("Load"))
            {
                var path = EditorUtility.OpenFilePanelWithFilters("Open DCTree JSON", "Assets", new[] {"JSON", "json"});

                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogWarning("User cancelled opening Tree");
                }
                else
                {
                    path = path.Replace(Application.dataPath, "Assets");

                    var nodes = BehaviorTree.LoadForEditor(path);
                    
                    _nodeWindows.Clear();

                    foreach (var serializableNode in nodes)
                    {
                        _nodeWindows.Add(new NodeWindow(serializableNode));
                    }

                    if (_nodeWindows.Count > 0)
                    {
                        _rootNode = _nodeWindows[0];
                    }

                    for (int i = 0; i < _nodeWindows.Count; i++)
                    {
                        var en = _nodeWindows[i].EditorNode;
                        for (int j = 0; j < en.NodeData.ChildCount; j++)
                        {
                            en.AddChild(_nodeWindows[en.NodeData.FirstChildIndex + j].EditorNode);
                        }
                    }
                }
            }

            if (_nodeWindows.Count > 0 && GUILayout.Button("Save"))
            {
                var path = EditorUtility.SaveFilePanelInProject("Save DCTree JSON", "NewDCTree", "json", "Save");

                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogWarning("User cancelled saving Tree");
                }
                else
                {
                    var nodes = new List<SerializableNode>();
                    OrderNodeList(nodes, _rootNode.EditorNode);

                    foreach (var nodeWindow in _nodeWindows)
                    {
                        if (!nodes.Contains(nodeWindow.EditorNode.NodeData))
                        {
                            nodes.Add(nodeWindow.EditorNode.NodeData);
                        }
                    }

                    BehaviorTree.Save(path, nodes);
                }
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Add Node"))
            {
                var nodeWin = new NodeWindow(typeof (FiniteRepeater), new Vector2(position.width / 2 - 200, 250));

                if (_nodeWindows.Count == 0)
                {
                    _rootNode = nodeWin;
                }

                _nodeWindows.Add(nodeWin);
            }

            if (_nodeWindows.Count == 0)
            {
                return;
            }

            var rootRect = new Rect(position.width / 2 - 100, 100, 50, 50);
            GUI.Box(rootRect, "Root");
            EditorGUIUtility.AddCursorRect(rootRect, MouseCursor.Link);

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (rootRect.Contains(Event.current.mousePosition))
                    {
                        _draggingRoot = true;
                        Event.current.Use();
                    }
                    break;
            }

            Handles.BeginGUI();

            if (_draggingRoot)
            {
                Handles.DrawLine(rootRect.center, Event.current.mousePosition);

                switch (Event.current.type)
                {
                    case EventType.MouseUp:
                        foreach (var nodeWindow in _nodeWindows)
                        {
                            if (nodeWindow.WindowRect.Contains(Event.current.mousePosition))
                            {
                                nodeWindow.EditorNode.RemoveParent();
                                _rootNode = nodeWindow;
                                break;
                            }
                        }
                        _draggingRoot = false;
                        break;
                    case EventType.MouseDrag:
                        Event.current.Use();
                        break;
                }
            }

            if (_rootNode != null)
            {
                Handles.DrawLine(rootRect.center, _rootNode.WindowRect.position + Vector2.right * _rootNode.WindowRect.width / 2);
            }

            foreach (var nodeWindow in _nodeWindows)
            {
                switch (nodeWindow.EditorNode.ChildType)
                {
                    case EditorNode.NodeChildType.None:
                        break;
                    case EditorNode.NodeChildType.Single:
                    case EditorNode.NodeChildType.Multiple:
                        for (int i = 0; i < nodeWindow.EditorNode.Children.Count; i++)
                        {
                            if (i >= nodeWindow.EditorNode.ConnectorPositions.Length)
                            {
                                continue;
                            }

                            var child = nodeWindow.EditorNode.Children[i];
                            Handles.DrawLine(
                                        nodeWindow.WindowRect.position + nodeWindow.EditorNode.ConnectorPositions[i],
                                        child.NodeData.EditorPosition +
                                        Vector2.right * nodeWindow.WindowRect.width / 2);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (_connectingWindow != null)
            {
                Handles.DrawLine(_connectingWindow.WindowRect.position + _connectingWindow.EditorNode.ConnectorPositions.Last(), Event.current.mousePosition);

                switch (Event.current.type)
                {
                    case EventType.MouseUp:
                        foreach (var nodeWindow in _nodeWindows)
                        {
                            if (nodeWindow != _connectingWindow && nodeWindow != _rootNode &&
                                nodeWindow.WindowRect.Contains(Event.current.mousePosition))
                            {
                                if (_connectingWindow.EditorNode.AddChild(nodeWindow.EditorNode))
                                {
                                    Debug.Log("connected");
                                }
                                Event.current.Use();
                                break;
                            }
                        }

                        _connectingWindow = null;
                        break;
                    case EventType.MouseDrag:
                        Event.current.Use();
                        break;
                }
            }

            Handles.EndGUI();


            BeginWindows();
            foreach (var nodeWindow in _nodeWindows)
            {
                nodeWindow.Draw();

                if (nodeWindow.GotMouseDownOnConnector)
                {
                    _connectingWindow = nodeWindow;
                }
            }
            EndWindows();
        }
    }
}