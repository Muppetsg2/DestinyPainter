using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ColorPalette.ColorEntry))]
public class ColorEntryDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            // 5 linii pól + 5 odstêpów + 1 linia foldoutu (nag³ówka)
            return EditorGUIUtility.singleLineHeight * 5 + EditorGUIUtility.standardVerticalSpacing * 5 + EditorGUIUtility.singleLineHeight;
        }
        else
        {
            // tylko 1 linia foldoutu (nag³ówka)
            return EditorGUIUtility.singleLineHeight;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, lineHeight),
            property.isExpanded,
            label,
            true
        );

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            float y = position.y + lineHeight + spacing;

            SerializedProperty colorTypeProp = property.FindPropertyRelative("colorType");
            SerializedProperty primaryColorProp = property.FindPropertyRelative("primaryColor");
            SerializedProperty secondaryColorProp = property.FindPropertyRelative("secondaryColor");
            SerializedProperty primaryHDRColorProp = property.FindPropertyRelative("primaryHDRColor");
            SerializedProperty secondaryHDRColorProp = property.FindPropertyRelative("secondaryHDRColor");

            Rect rect = new Rect(position.x, y, position.width, lineHeight);
            EditorGUI.PropertyField(rect, colorTypeProp);
            y += lineHeight + spacing;

            rect = new Rect(position.x, y, position.width, lineHeight);
            EditorGUI.PropertyField(rect, primaryColorProp);
            y += lineHeight + spacing;

            rect = new Rect(position.x, y, position.width, lineHeight);
            EditorGUI.PropertyField(rect, secondaryColorProp);
            y += lineHeight + spacing;

            rect = new Rect(position.x, y, position.width, lineHeight);
            EditorGUI.PropertyField(rect, primaryHDRColorProp);
            y += lineHeight + spacing;

            rect = new Rect(position.x, y, position.width, lineHeight);
            EditorGUI.PropertyField(rect, secondaryHDRColorProp);

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }
}
