using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace DerelictComputer
{
    public class PercolatorEditorWindow : EditorWindow
    {
        private readonly List<Rect> _windowRects = new List<Rect>();

        [MenuItem("Window/Percolator")]
        private static void Init()
        {
            GetWindow<PercolatorEditorWindow>().Show();
        }

        private void OnEnable()
        {
            if (_windowRects.Count == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    _windowRects.Add(new Rect(i * 100, 0, 100, 50));
                }
            }
        }

        private void OnGUI()
        {
            Handles.BeginGUI();
            for (int i = 0; i < _windowRects.Count - 1; i++)
            {
                Handles.DrawBezier(_windowRects[i].center, _windowRects[i + 1].center, new Vector2(_windowRects[i].xMax + 50f, _windowRects[i].center.y), new Vector2(_windowRects[i + 1].xMin - 50f, _windowRects[i + 1].center.y), Color.red, null, 5f);
            }
            Handles.EndGUI();

            BeginWindows();
            for (int i = 0; i < _windowRects.Count; i++)
            {
                _windowRects[i] = GUI.Window(i, _windowRects[i], OnWindowUpdate, "Window " + i);
            }
            EndWindows();
        }

        private void OnWindowUpdate(int windowId)
        {
            GUI.DragWindow();
        }
    }
}