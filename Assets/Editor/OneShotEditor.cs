using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace DerelictComputer
{
    public class OneShotEditor : EditorWindow
    {
        private OneShot _oneShot;
        private GUISkin _guiSkin;
        private readonly Dictionary<OneShotSampleConfig, OneShotSampleBox> _boxen = new Dictionary<OneShotSampleConfig, OneShotSampleBox>();
        
        [MenuItem("Window/One Shot Editor")]
        private static void Init()
        {
            var w = GetWindow<OneShotEditor>();
            w.titleContent = new GUIContent("One Shot");
            w.Show();
        }

        private void UpdateState()
        {
            _oneShot = Selection.activeGameObject != null ? Selection.activeGameObject.GetComponent<OneShot>() : null;
            _guiSkin = Resources.Load<GUISkin>("OneShotEditorSkin");
            _boxen.Clear();
        }

        private void OnSelectionChange()
        {
            UpdateState();
            Repaint();
        }

        private void OnFocus()
        {
            UpdateState();
        }

        private void OnGUI()
        {
            if (_oneShot == null)
            {
                EditorGUI.LabelField(new Rect(Vector2.zero, position.size), "Select a OneShot to edit it.");
                return;
            }

            const float bumperWidth = 16;
            const float pianoKeysHeight = 40;

            float sampleAreaWidth = position.width - bumperWidth*2;
            float sampleAreaHeight = position.height - pianoKeysHeight;

            DrawSampleArea(new Rect(bumperWidth, 0, sampleAreaWidth, sampleAreaHeight));

            GUI.Box(new Rect(bumperWidth, sampleAreaHeight, sampleAreaWidth, pianoKeysHeight), "", _guiSkin.GetStyle("PianoKeysBox"));
        }

        private void DrawSampleArea(Rect rect)
        {
            const float minRowHeight = 40;
            const float rowPadding = 4;

            float vOffset = rowPadding;
            float dragRowHeight = _oneShot.Samples.Count == 0 ? rect.height : minRowHeight;

            DrawSampleAddTarget(new Rect(rect.x, rect.y + vOffset, rect.width, dragRowHeight));

            vOffset += dragRowHeight + rowPadding;

            float rowHeight = Mathf.Max((rect.height - dragRowHeight - rowPadding * (_oneShot.Samples.Count + 2)) / _oneShot.Samples.Count, minRowHeight);

            for (int i = 0; i < _oneShot.Samples.Count; i++)
            {
                OneShotSampleBox box;

                if (!_boxen.TryGetValue(_oneShot.Samples[i], out box))
                {
                    box = new OneShotSampleBox(_oneShot.Samples[i]);
                    _boxen.Add(_oneShot.Samples[i], box);
                }

                box.Draw(new Rect(rect.x, rect.y + vOffset, rect.width, rowHeight), _guiSkin);
                vOffset += rowHeight + rowPadding;
            }
        }

        private void DrawSampleAddTarget(Rect rect)
        {
            GUI.Box(rect, "Drag AudioClips Here");

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    }
                    break;
                case EventType.DragPerform:
                    DragAndDrop.AcceptDrag();

                    foreach (var objectReference in DragAndDrop.objectReferences)
                    {
                        AudioClip clip = objectReference as AudioClip;
                        if (clip != null)
                        {
                            var config = OneShotSampleConfig.Default;
                            config.Clip = clip;
                            _oneShot.Samples.Add(config);
                        }
                    }
                    break;
            }
        }
    }
}