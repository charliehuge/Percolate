using System;
using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class NodeWindow
    {
        private static readonly Vector2 WindowSize = new Vector2(150, 60);
        private static int _nextId;

        public readonly EditorNode EditorNode;

        private readonly int _id;

        public Rect WindowRect { get; private set; }
        public bool GotMouseDownOnConnector { get; private set; }

        public NodeWindow(Type type, Vector2 position)
        {
            EditorNode = new EditorNode(type);
            WindowRect = new Rect(position, WindowSize);
            _id = _nextId++;
        }

        public NodeWindow(SerializableNode sNode)
        {
            EditorNode = new EditorNode(sNode);
            WindowRect = new Rect(EditorNode.NodeData.EditorPosition, WindowSize);
            _id = _nextId++;
        }

        public void Draw()
        {
            WindowRect = GUI.Window(_id, WindowRect, WindowUpdate, EditorNode.DisplayName);
            EditorNode.NodeData.EditorPosition = WindowRect.position;
        }

        private void WindowUpdate(int id)
        {
            GotMouseDownOnConnector = EditorNode.DrawEditor(WindowRect);
            GUI.DragWindow();
        }
    }
}