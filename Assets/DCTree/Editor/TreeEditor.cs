using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class TreeEditor : EditorWindow
    {
        private readonly List<NodeWindow> _nodeWindows = new List<NodeWindow>();
        private TreeInfo _treeInfo;
        private NodeWindow _connectingWindow;

        [MenuItem("Window/DCTree Editor")]
        private static void Init()
        {
            GetWindow<TreeEditor>().Show();
        }

        private void OnEnable()
        {
            _treeInfo = null;
            Reset();
        }

        private void OnFocus()
        {
            Reset();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("New Tree"))
            {
                _treeInfo = CreateInstance<TreeInfo>();
                Reset();
            }

            if (_treeInfo == null)
            {
                return;
            }

            if (GUILayout.Button("Add Node"))
            {
                var nodeInfo = new NodeInfo(typeof (Sequence));
                _treeInfo.AllNodes.Add(nodeInfo);
                _nodeWindows.Add(new NodeWindow(nodeInfo));
            }

            Handles.BeginGUI();

            if (_connectingWindow != null)
            {
                Handles.DrawLine(_connectingWindow.WindowRect.position + _connectingWindow.NodeInfo.ChildParam.ConnectorPositions.Last(), Event.current.mousePosition);

                switch (Event.current.type)
                {
                    case EventType.MouseUp:
                        foreach (var nodeWindow in _nodeWindows)
                        {
                            if (nodeWindow != _connectingWindow &&
                                nodeWindow.WindowRect.Contains(Event.current.mousePosition))
                            {
                                if (_connectingWindow.NodeInfo.AddChild(nodeWindow.NodeInfo))
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

            foreach (var nodeWindow in _nodeWindows)
            {
                if (nodeWindow.NodeInfo.ChildParam == null)
                {
                    continue;
                }

                var children = nodeWindow.NodeInfo.ChildParam.GetChildren();
                var connectors = nodeWindow.NodeInfo.ChildParam.ConnectorPositions;

                if (connectors == null)
                {
                    continue;
                }

                for (int i = 0; i < connectors.Length; i++)
                {
                    if (i >= children.Length || children[i] == null)
                    {
                        continue;
                    }

                    Handles.DrawLine(nodeWindow.WindowRect.position + connectors[i], children[i].EditorPosition + Vector2.right * nodeWindow.WindowRect.width /2);
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

        private void Reset()
        {
            _nodeWindows.Clear();

            if (_treeInfo == null)
            {
                return;
            }

            foreach (var nodeInfo in _treeInfo.AllNodes)
            {
                _nodeWindows.Add(new NodeWindow(nodeInfo));
            }
        }
    }
}