using System;
using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class NodeWindow
    {
        private static readonly Vector2 WindowSize = new Vector2(300, 80);
        private static int _nextId;

        public readonly NodeInfo NodeInfo;

        private readonly int _id;

        public Rect WindowRect { get; private set; }
        public bool GotMouseDownOnConnector { get; private set; }

        public NodeWindow(NodeInfo nodeInfo)
        {
            WindowRect = new Rect(nodeInfo.EditorPosition, WindowSize);
            _id = _nextId++;
            NodeInfo = nodeInfo;
        }

        public void Draw()
        {
            WindowRect = GUI.Window(_id, WindowRect, WindowUpdate, NodeInfo.NodeType.Name);
            NodeInfo.EditorPosition = WindowRect.position;
        }

        private void WindowUpdate(int id)
        {
            GotMouseDownOnConnector = NodeInfo.DrawEditor(WindowRect);
            GUI.DragWindow();
        }
    }
}