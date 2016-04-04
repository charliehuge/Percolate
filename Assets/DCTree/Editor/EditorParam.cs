using UnityEditor;
using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class EditorParam
    {
        public static void DrawEditor(SerializableNodeParam param)
        {
            EditorGUIUtility.labelWidth = 60;
            if (param.Type == typeof (float))
            {
                if (param.Min != null && param.Max != null)
                {
                    param.FloatValue = EditorGUILayout.Slider(param.FieldName, param.FloatValue, (float)param.Min, (float)param.Max);
                }
                else
                {
                    param.FloatValue = EditorGUILayout.FloatField(param.FieldName, param.FloatValue);
                }
            }
            else if (param.Type == typeof (int))
            {
                if (param.Min != null && param.Max != null)
                {
                    param.IntValue = EditorGUILayout.IntSlider(param.FieldName, param.IntValue, (int)param.Min, (int)param.Max);
                }
                else
                {
                    param.IntValue = EditorGUILayout.IntField(param.FieldName, param.IntValue);
                }
            }
            else if (param.Type == typeof (string))
            {
                param.StringValue = EditorGUILayout.TextField(param.FieldName, param.StringValue);
            }
            EditorGUIUtility.labelWidth = 0;
        }
    }
}
