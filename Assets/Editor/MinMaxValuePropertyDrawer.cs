using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(MinMaxValue<>))]
public class MinMaxValuePropertyDrawer : PropertyDrawer
{
    private const float LABEL_HEIGHT = 16f;

    private SerializedProperty min;
    private SerializedProperty max;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (min == null)
        {
            min = property.FindPropertyRelative("Min");
        }

        if (max == null)
        {
            max = property.FindPropertyRelative("Max");
        }

        return LABEL_HEIGHT + EditorGUI.GetPropertyHeight(min);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect labelRect = new (position.x, position.y, position.width, LABEL_HEIGHT);
        EditorGUI.LabelField(labelRect, label);

        EditorGUI.indentLevel++;
        float width = position.width * 0.5f;
        
        Rect minRect = new (position.x, position.y + LABEL_HEIGHT, width, EditorGUI.GetPropertyHeight(min));
        EditorGUI.PropertyField(minRect, min);

        Rect maxRect = new (position.x + width, position.y + LABEL_HEIGHT, width, EditorGUI.GetPropertyHeight(max));
        EditorGUI.PropertyField(maxRect, max);

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}
