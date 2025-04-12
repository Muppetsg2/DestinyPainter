using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ShowInInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // standardowe pola

        var targetType = target.GetType();
        var properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            var attr = prop.GetCustomAttribute<ShowInInspectorAttribute>();
            if (attr == null || !prop.CanRead || !prop.CanWrite)
                continue;

            Type type = prop.PropertyType;
            object value = prop.GetValue(target);

            EditorGUI.BeginChangeCheck();
            object newValue = null;

            if (type == typeof(int))
            {
                newValue = EditorGUILayout.IntField(prop.Name, (int)value);
            }
            else if (type == typeof(float))
            {
                newValue = EditorGUILayout.FloatField(prop.Name, (float)value);
            }
            else if (type == typeof(bool))
            {
                newValue = EditorGUILayout.Toggle(prop.Name, (bool)value);
            }
            else if (type == typeof(string))
            {
                newValue = EditorGUILayout.TextField(prop.Name, (string)value);
            }
            else if (type == typeof(Vector2))
            {
                newValue = EditorGUILayout.Vector2Field(prop.Name, (Vector2)value);
            }
            else if (type == typeof(Vector3))
            {
                newValue = EditorGUILayout.Vector3Field(prop.Name, (Vector3)value);
            }
            else if (type == typeof(Vector4))
            {
                newValue = EditorGUILayout.Vector4Field(prop.Name, (Vector4)value);
            }
            else if (type == typeof(Color))
            {
                newValue = EditorGUILayout.ColorField(prop.Name, (Color)value);
            }
            else if (type == typeof(Quaternion))
            {
                Quaternion quat = (Quaternion)value;
                Vector4 vec = new Vector4(quat.x, quat.y, quat.z, quat.w);
                Vector4 newVec = EditorGUILayout.Vector4Field(prop.Name + " (Quaternion)", vec);
                newValue = new Quaternion(newVec.x, newVec.y, newVec.z, newVec.w);
            }
            else if (type.IsEnum)
            {
                newValue = EditorGUILayout.EnumPopup(prop.Name, (Enum)value);
            }
            else
            {
                EditorGUILayout.LabelField(prop.Name, $"[Unsupported type: {type.Name}]");
            }

            if (EditorGUI.EndChangeCheck() && newValue != null)
            {
                Undo.RecordObject(target, "Change " + prop.Name);
                prop.SetValue(target, newValue);
                EditorUtility.SetDirty(target);
            }
        }
    }
}