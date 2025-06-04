using System;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MaterialPresetDropdownAttribute))]
public class MaterialPresetDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MaterialPresetDropdownAttribute attr = (MaterialPresetDropdownAttribute)attribute;
        SerializedProperty fontAssetProp = property.serializedObject.FindProperty(attr.FontAssetFieldName);

        TMP_FontAsset fontAsset = fontAssetProp?.objectReferenceValue as TMP_FontAsset;

        Material[] materials;
        string[] options;

        if (fontAsset != null)
        {
            string fontPath = AssetDatabase.GetAssetPath(fontAsset);
            string folderPath = System.IO.Path.GetDirectoryName(fontPath).Replace("\\", "/") + "/";
            var assetGuids = AssetDatabase.FindAssets("", new[] { folderPath });
            var assetPaths = assetGuids.Select(guid => AssetDatabase.GUIDToAssetPath(guid));

            string[] materialsPaths = assetPaths.Where(a =>
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(a);
                if (mat == null || !mat.mainTexture.Equals(fontAsset.atlasTexture)) return false;

                int dashIndex = fontAsset.name.IndexOf('-');
                string fontName = fontAsset.name;
                if (dashIndex != -1) fontName = fontName[..dashIndex].TrimEnd();

                return mat.name.StartsWith(fontName);
            }).ToArray();

            materials = materialsPaths.Select(mp => AssetDatabase.LoadAssetAtPath<Material>(mp)).ToArray();
            options = materials.Select(m => m.name).ToArray();
        }
        else
        {
            materials = Array.Empty<Material>();
            options = new string[] { "None (no font asset)" };
        }

        int currentIndex = 0;

        if (property.objectReferenceValue is Material currentMat && materials.Length > 0)
        {
            currentIndex = Array.IndexOf(materials, currentMat);
            if (currentIndex < 0) currentIndex = 0;
        }

        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, options);

        if (materials.Length > 0 && newIndex < materials.Length)
        {
            property.objectReferenceValue = materials[newIndex];
        }
        else
        {
            property.objectReferenceValue = null;
        }
    }
}