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
        private OneShotSampleConfig _currentConfig;
        
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
            _currentConfig = null;
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

            const float bumperWidth = 8;
            const float pianoKeysHeight = 40;
            float sampleEditorHeight = _currentConfig != null ? 130 : 0;

            float sampleAreaWidth = position.width - bumperWidth*2;
            float sampleAreaHeight = position.height - pianoKeysHeight - sampleEditorHeight;


            DrawSampleArea(new Rect(bumperWidth, 0, sampleAreaWidth, sampleAreaHeight));

            DrawPianoKeysArea(new Rect(bumperWidth, sampleAreaHeight, sampleAreaWidth, pianoKeysHeight));

            if (_currentConfig != null)
            {
                DrawSampleEditor(new Rect(bumperWidth, sampleAreaHeight + pianoKeysHeight, sampleAreaWidth, sampleEditorHeight));
            }
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
            var sampleActions = new OneShotSampleBox.SampleAction[_oneShot.Samples.Count];

            for (int i = 0; i < _oneShot.Samples.Count; i++)
            {
                OneShotSampleBox box;

                if (!_boxen.TryGetValue(_oneShot.Samples[i], out box))
                {
                    box = new OneShotSampleBox(_oneShot.Samples[i]);
                    _boxen.Add(_oneShot.Samples[i], box);
                }

                sampleActions[i] = box.Draw(new Rect(rect.x, rect.y + vOffset, rect.width, rowHeight), _guiSkin);
                vOffset += rowHeight + rowPadding;
            }

            var samplesToDelete = new List<OneShotSampleConfig>();

            for (int i = 0; i < sampleActions.Length; i++)
            {
                switch (sampleActions[i])
                {
                    case OneShotSampleBox.SampleAction.None:
                        break;
                    case OneShotSampleBox.SampleAction.Delete:
                        samplesToDelete.Add(_oneShot.Samples[i]);
                        break;
                    case OneShotSampleBox.SampleAction.Edit:
                        _currentConfig = _oneShot.Samples[i];
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            for (int i = 0; i < samplesToDelete.Count; i++)
            {
                if (_currentConfig == samplesToDelete[i])
                {
                    _currentConfig = null;
                }
                _oneShot.Samples.Remove(samplesToDelete[i]);
            }
        }

        private void DrawSampleEditor(Rect rect)
        {
            GUILayout.BeginArea(rect);
            EditorGUILayout.LabelField(_currentConfig.Clip.name + " Settings");
            EditorGUILayout.Space();
            _currentConfig.ScalePitch = EditorGUILayout.Toggle("Scale Pitch", _currentConfig.ScalePitch);
            _currentConfig.AttackTime = EditorGUILayout.Slider("Attack", (float)_currentConfig.AttackTime, 0, 2);
            _currentConfig.SustainTime = EditorGUILayout.Slider("Sustain", (float)_currentConfig.SustainTime, 0, 2);
            _currentConfig.ReleaseTime = EditorGUILayout.Slider("Release", (float)_currentConfig.ReleaseTime, 0, 2);
            GUILayout.EndArea();
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

        private void DrawPianoKeysArea(Rect rect)
        {
            GUI.Box(rect, "", _guiSkin.GetStyle("PianoKeysBox"));

            const int numKeys = 128;

            if (Application.isPlaying)
            {
                for (int i = 0; i < numKeys; i++)
                {
                    float w = rect.width/numKeys;
                    float x = rect.x + i*w;
                    Rect r = new Rect(x, rect.y, w, rect.height);
                    switch (Event.current.type)
                    {
                        case EventType.MouseUp:
                            if (r.Contains(Event.current.mousePosition))
                            {
                                _oneShot.Play(AudioSettings.dspTime, i);
                                Event.current.Use();
                            }
                            break;
                    }
                }
            }
        }
    }
}