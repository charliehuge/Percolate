using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DerelictComputer
{
    public class OneShotSampleBox
    {
        private readonly OneShotSampleConfig _config;

        private float _dragStartX;
        private int _dragStartBottomNote;
        private int _dragStartTopNote;
        private bool _moving;
        private bool _resizingLeft;
        private bool _resizingRight;

        public OneShotSampleBox(OneShotSampleConfig config)
        {
            _config = config;
        }

        public void Draw(Rect boundsRect, GUISkin skin)
        {
            const float resizeWidth = 20;

            float x = boundsRect.x + boundsRect.width * _config.BottomNote/127f;
            float w = boundsRect.width * (_config.TopNote- _config.BottomNote)/127f;
            Rect rect = new Rect(x, boundsRect.y, w, boundsRect.height);
            Rect resizeRectLeft = new Rect(rect.x, rect.y, resizeWidth, rect.height);
            Rect resizeRectRight = new Rect(rect.x + rect.width - resizeWidth, rect.y, resizeWidth, rect.height);
            Rect moveRect = new Rect(rect.x + resizeWidth, rect.y, rect.width - resizeWidth * 2, rect.height);

            EditorGUIUtility.AddCursorRect(resizeRectLeft, MouseCursor.ResizeHorizontal);
            EditorGUIUtility.AddCursorRect(resizeRectRight, MouseCursor.ResizeHorizontal);
            EditorGUIUtility.AddCursorRect(moveRect, MouseCursor.MoveArrow);

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (resizeRectLeft.Contains(Event.current.mousePosition))
                    {
                        _resizingLeft = true;
                        _dragStartX = Event.current.mousePosition.x;
                        _dragStartBottomNote = _config.BottomNote;
                        _dragStartTopNote = _config.TopNote;
                        Event.current.Use();
                    }
                    else if (resizeRectRight.Contains(Event.current.mousePosition))
                    {
                        _resizingRight = true;
                        _dragStartX = Event.current.mousePosition.x;
                        _dragStartBottomNote = _config.BottomNote;
                        _dragStartTopNote = _config.TopNote;
                        Event.current.Use();
                    }
                    else if (moveRect.Contains(Event.current.mousePosition))
                    {
                        _moving = true;
                        _dragStartX = Event.current.mousePosition.x;
                        _dragStartBottomNote = _config.BottomNote;
                        _dragStartTopNote = _config.TopNote;
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    _resizingRight = false;
                    _resizingLeft = false;
                    _moving = false;
                    break;
                case EventType.MouseDrag:
                    if (_moving)
                    {
                        float delta = Event.current.mousePosition.x - _dragStartX;
                        int noteDelta = (int)(127*delta/boundsRect.width);
                        int newBottom = _dragStartBottomNote + noteDelta;
                        int newTop = _dragStartTopNote + noteDelta;

                        if (newBottom < 0)
                        {
                            _config.BottomNote = 0;
                            _config.TopNote = newTop - newBottom;
                        }
                        else if (newTop > 127)
                        {
                            _config.TopNote = 127;
                            _config.BottomNote = newBottom + 127 - newTop;
                        }
                        else
                        {
                            _config.BottomNote = newBottom;
                            _config.TopNote = newTop;
                        }
                        Event.current.Use();
                    }
                    else if (_resizingLeft)
                    {
                        float delta = Event.current.mousePosition.x - _dragStartX;
                        int noteDelta = (int)(127 * delta / boundsRect.width);
                        int newBottom = _dragStartBottomNote + noteDelta;
                        _config.BottomNote = Mathf.Clamp(newBottom, 0, _dragStartTopNote);
                        Event.current.Use();
                    }
                    else if (_resizingRight)
                    {
                        float delta = Event.current.mousePosition.x - _dragStartX;
                        int noteDelta = (int)(127 * delta / boundsRect.width);
                        int newTop = _dragStartTopNote + noteDelta;
                        _config.TopNote = Mathf.Clamp(newTop, _dragStartBottomNote, 127);
                        Event.current.Use();
                    }
                    break;
            }

            GUI.Box(rect, _config.Clip.name + " (" + _config.BottomNote + ", " + _config.TopNote + ")", skin.GetStyle("ClipBox"));
        }
    }
}
