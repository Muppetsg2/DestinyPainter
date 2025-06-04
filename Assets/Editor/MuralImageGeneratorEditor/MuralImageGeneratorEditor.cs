using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MuralImageGeneratorEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    // Bindings
    [SerializeField]
    private Texture2D m_previewTexture;
    // B_text
    [SerializeField]
    private string m_text;
    [SerializeField]
    private TMP_FontAsset m_font;
    [SerializeField]
    private Material m_presetMaterial;
    [SerializeField]
    private int m_textSize;
    [SerializeField]
    private Color m_textColor;
    // B_background
    [SerializeField]
    private float m_backgroundRadius;
    [SerializeField]
    private Color m_backgroundColor;
    [SerializeField]
    private float m_backgroundBorder;
    // B_splash
    [SerializeField]
    private int m_numberOfSplashes;
    [SerializeField]
    private GameObject m_splashPrefab;
    [SerializeField]
    private float m_splashAlpha;
    [SerializeField]
    private float m_splashEdgeSharpness;
    [SerializeField]
    private float m_splashScale;
    [SerializeField]
    private float m_splashFillScale;
    // B_random_scalers
    [SerializeField]
    private float m_circleAngleRandomScaler;
    [SerializeField]
    private float m_circleDistanceRandomScaler;
    // B_splash_colors
    [SerializeField]
    private List<Color> m_colors;

    // Preview
    private int m_previewSize = 512;
    private Scene m_previewScene;
    private GameObject m_cameraObject;
    private Camera m_sceneCamera;
    private GameObject m_lightObject;

    // Buttons
    private UnityEngine.UIElements.Button m_generateButton;
    private UnityEngine.UIElements.Button m_saveButton;
    private UnityEngine.UIElements.Button m_loadSetButton;
    private UnityEngine.UIElements.Button m_saveSetButton;

    // Dropdown
    private List<Material> m_materials;
    private DropdownField m_materialsDropdown;

    // List
    private ListView m_colorsList;

    // Generated Root
    private GameObject m_generatedRoot;

    // Background Color
    private Color bgColor;

    // Random
    private System.Random rng = new System.Random();

    [MenuItem("Tools/Mural Image Generator Editor")]
    public static void ShowExample()
    {
        MuralImageGeneratorEditor wnd = GetWindow<MuralImageGeneratorEditor>();
        wnd.titleContent = new GUIContent("Mural Image Generator Editor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        VisualElement container = rootVisualElement.Q<VisualElement>("Container");
        container.dataSource = this;
        bgColor = container.style.backgroundColor.value;

        // Get Elements
        m_saveButton = rootVisualElement.Q<UnityEngine.UIElements.Button>("SaveImage");
        m_saveButton.clicked += Export;
        m_generateButton = rootVisualElement.Q<UnityEngine.UIElements.Button>("Generate");
        m_generateButton.clicked += Generate;

        m_loadSetButton = rootVisualElement.Q<UnityEngine.UIElements.Button>("LoadSettings");
        m_loadSetButton.clicked += LoadData;
        m_saveSetButton = rootVisualElement.Q<UnityEngine.UIElements.Button>("SaveSettings");
        m_saveSetButton.clicked += SaveData;

        m_materialsDropdown = rootVisualElement.Q<DropdownField>("MaterialsDropdown");
        RefreshDropdown(m_font, m_presetMaterial);

        m_materialsDropdown.RegisterValueChangedCallback(evt =>
        {
            int newIndex = Array.IndexOf(m_materialsDropdown.choices.ToArray(), evt.newValue);
            if (newIndex >= 0 && newIndex < m_materials.Count)
            {
                m_presetMaterial = m_materials[newIndex];
            }
            else
            {
                m_presetMaterial = null;
            }
        });

        ObjectField fontField = rootVisualElement.Q<ObjectField>("Font");
        fontField.RegisterValueChangedCallback((evt) =>
        {
            TMP_FontAsset font = evt.newValue as TMP_FontAsset;
            RefreshDropdown(font, m_presetMaterial);
        });

        m_colors = new List<Color>();
        m_colorsList = rootVisualElement.Q<ListView>("ColorList");
        m_colorsList.itemsSource = m_colors;
        m_colorsList.makeItem = () =>
        {
            VisualElement elem = new VisualElement();
            GroupBox box = new GroupBox();
            box.Add(new Label($"Color {m_colors.Count}"));
            box.Add(new ColorField());
            elem.Add(box);

            return elem;
        };
        m_colorsList.bindItem = (e, i) =>
        {
            Label label = e.Q<Label>();
            label.text = $"Color {i}";

            ColorField colorField = e.Q<ColorField>();

            if (colorField.userData != null)
            {
                colorField.UnregisterValueChangedCallback(colorField.userData as EventCallback<ChangeEvent<Color>>);
                colorField.userData = null;
            }

            colorField.value = m_colors[i];

            EventCallback<ChangeEvent<Color>> callback = evt => m_colors[i] = evt.newValue;
            colorField.RegisterValueChangedCallback(callback);
            colorField.userData = callback;
        };
        m_colorsList.destroyItem = (e) =>
        {
            ColorField colorField = e.Q<ColorField>();
            colorField.UnregisterValueChangedCallback(colorField.userData as EventCallback<ChangeEvent<Color>>);
            colorField.userData = null;
            colorField.ClearBindings();
        };

        InitScene();
    }

    private void OnDestroy()
    {
        if (m_generatedRoot != null)
        {
            DestroyImmediate(m_generatedRoot);
            m_generatedRoot = null;
        }

        if (m_materials.Count > 0) 
        {
            m_materials.Clear();
        }

        if (m_colors.Count > 0)
        {
            m_colors.Clear();
        }

        if (m_cameraObject != null)
        {
            DestroyImmediate(m_cameraObject);
            m_cameraObject = null;
        }

        if (m_lightObject != null)
        {
            DestroyImmediate(m_lightObject);
            m_lightObject = null;
        }

        if (m_previewScene.IsValid())
        {
            EditorSceneManager.CloseScene(m_previewScene, true);
        }
    }

    private void RefreshDropdown(TMP_FontAsset font, Material presetMaterial = null)
    {
        if (m_materials == null) m_materials = new List<Material>();
        else m_materials.Clear();

        m_materials = GetFontMaterials(font);
        List<string> options = m_materials.Count > 0 ?
            m_materials.Select(m => m.name).ToList() :
            new List<string> { "None" };
        m_materialsDropdown.choices = options;

        if (presetMaterial != null)
        {
            int newIndex = Array.IndexOf(m_materialsDropdown.choices.ToArray(), presetMaterial.name);
            if (newIndex >= 0 && newIndex < m_materials.Count)
            {
                m_materialsDropdown.index = newIndex;
                m_materialsDropdown.value = options[newIndex];
                m_presetMaterial = m_materials[newIndex];
                return;
            }
        }

        m_materialsDropdown.index = 0;
        m_materialsDropdown.value = options[0];
        m_presetMaterial = m_materials.Count > 0 ? m_materials[0] : null;
    }

    private List<Material> GetFontMaterials(TMP_FontAsset font)
    {
        List<Material> materials = new List<Material>();

        if (font != null)
        {
            string fontPath = AssetDatabase.GetAssetPath(font);
            string folderPath = Path.GetDirectoryName(fontPath).Replace("\\", "/") + "/";
            var assetGuids = AssetDatabase.FindAssets("", new[] { folderPath });
            var assetPaths = assetGuids.Select(guid => AssetDatabase.GUIDToAssetPath(guid));

            string[] materialsPaths = assetPaths.Where(a =>
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(a);
                if (mat == null || !mat.mainTexture.Equals(font.atlasTexture)) return false;

                int dashIndex = font.name.IndexOf('-');
                string fontName = font.name;
                if (dashIndex != -1) fontName = fontName[..dashIndex].TrimEnd();

                return mat.name.StartsWith(fontName);
            }).ToArray();

            materials = materialsPaths.Select(mp => AssetDatabase.LoadAssetAtPath<Material>(mp)).ToList();
        }

        return materials;
    }

    private void InitScene()
    {
        if (!m_previewScene.IsValid())
        {
            m_previewScene = EditorSceneManager.NewPreviewScene();
        }

        if (m_cameraObject == null)
        {
            m_cameraObject = new GameObject("Camera");
            m_cameraObject.transform.eulerAngles = Vector3.zero;
            m_cameraObject.transform.position = new Vector3(0, 0, -1);

            m_sceneCamera = m_cameraObject.AddComponent<Camera>();
            m_sceneCamera.aspect = 1;
            m_sceneCamera.orthographic = true;
            m_sceneCamera.orthographicSize = 1;
            m_sceneCamera.backgroundColor = bgColor;
            m_sceneCamera.clearFlags = CameraClearFlags.SolidColor;
            m_sceneCamera.targetTexture = new RenderTexture(m_previewSize, m_previewSize, 32, RenderTextureFormat.ARGBFloat);

            SceneManager.MoveGameObjectToScene(m_cameraObject, m_previewScene);

            m_sceneCamera.scene = m_previewScene;
        }

        if (m_lightObject == null)
        {
            m_lightObject = new GameObject("GlobalLight2D");
            SceneManager.MoveGameObjectToScene(m_lightObject, m_previewScene);
            Light2D l2d = m_lightObject.AddComponent<Light2D>();
            l2d.color = Color.white;
            l2d.intensity = 1;
            l2d.pointLightOuterRadius = 2;
            l2d.pointLightInnerRadius = 2;
        }

        UpdateCamera();
    }

    private void UpdateCamera(bool toPNG = false)
    {
        m_sceneCamera.Render();

        if (m_previewTexture == null)
        {
            m_previewTexture = new Texture2D(m_previewSize, m_previewSize, TextureFormat.RGBAFloat, false, true);
        }

        RenderTexture last = RenderTexture.active;
        RenderTexture.active = m_sceneCamera.targetTexture;

        m_previewTexture.ReadPixels(new Rect(0, 0, m_previewSize, m_previewSize), 0, 0);

        ApplyCircularMask(m_previewTexture, m_backgroundRadius, toPNG ? new Color(0, 0, 0, 0) : bgColor);

        m_previewTexture.Apply();

        RenderTexture.active = last;
    }

    private void ApplyCircularMask(Texture2D texture, float radius, Color outsideColor)
    {
        int width = texture.width;
        int height = texture.height;
        Vector2 center = new Vector2(width / 2f, height / 2f);
        float radiusSqr = radius * radius;

        for (int y = 0; y < height; ++y)
        {
            float dy = y - center.y;
            float dySqr = dy * dy;
            for (int x = 0; x < width; ++x)
            {
                float dx = x - center.x;
                float distanceSqr = dx * dx + dySqr;

                if (distanceSqr > radiusSqr)
                {
                    texture.SetPixel(x, y, outsideColor);
                }
            }
        }
    }

    private void Export()
    {
        m_sceneCamera.depthTextureMode = DepthTextureMode.Depth;
        //m_sceneCamera.backgroundColor = new Color(0, 0, 0, 0);

        UpdateCamera(true);
        SaveTextureAsPNG(m_previewTexture, "GeneratedImage");

        //m_sceneCamera.backgroundColor = bgColor;

        UpdateCamera();
    }

    private void SaveTextureAsPNG(Texture2D texture, string filename)
    {
        if (texture == null)
        {
            Debug.LogError("No texture provided to save.");
            return;
        }

        string path = EditorUtility.SaveFilePanel("Save Texture As PNG", "", $"{filename}.png", "png");

        if (string.IsNullOrEmpty(path)) 
        {
            Debug.Log("Save operation cancelled.");
            return;
        }

        byte[] pngData = texture.EncodeToPNG();
        if (pngData == null) 
        {
            Debug.LogError("Failed to encode texture to PNG.");
            return;
        }

        File.WriteAllBytes(path, pngData);
        Debug.Log("Texture saved to: " + path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
    }

    private void Generate()
    {
        ClearScene();

        m_generatedRoot = new GameObject("Mural");
        m_generatedRoot.transform.position = Vector3.zero;
        m_generatedRoot.transform.localScale = Vector3.one;

        // Background
        m_sceneCamera.backgroundColor = m_backgroundColor;

        // Splashes
        float bgRadius = m_backgroundRadius - m_splashScale * 256f * 0.5f;
        float radius = (bgRadius * (1f - (m_backgroundBorder * 0.01f))).Remap(0f, bgRadius, 0f, 1f);
        int baseIndex = rng.Next(m_colors.Count);
        for (int i = 0; i < m_numberOfSplashes; ++i)
        {
            Color c = Color.white;
            if (m_colors.Count > 0)
            {
                int indexOffset = rng.Next(-1, 2); // -1, 0, 1
                int colorIndex = (baseIndex + i + indexOffset + m_colors.Count) % m_colors.Count;

                c = m_colors[colorIndex];
                c.a = m_splashAlpha;
            }

            Vector3 pos = GetRandomPointOnCircle(Vector3.zero, radius);
            GameObject splash = CreateSplash(
                m_generatedRoot.transform,
                pos,
                Vector3.one * m_splashScale,
                c,
                $"Splash_{i}"
            );
        }

        // Canvas
        GameObject canvasObj = new GameObject("Canvas");
        canvasObj.transform.SetParent(m_generatedRoot.transform);
        canvasObj.transform.position = Vector3.zero;
        canvasObj.transform.localScale = Vector3.one * 0.01f;
        canvasObj.AddComponent<RectTransform>();
        Vector2 size = new Vector2(m_backgroundRadius * radius, m_backgroundRadius * radius);
        canvasObj.GetComponent<RectTransform>().sizeDelta = size;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 0;

        // Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(canvasObj.transform);
        textObj.transform.position = Vector3.zero;
        textObj.transform.localScale = Vector3.one;
        textObj.AddComponent<RectTransform>().sizeDelta = size;

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = m_text;
        text.font = m_font;
        text.fontSharedMaterial = m_presetMaterial;
        text.fontSize = m_textSize;
        text.enableAutoSizing = false;
        text.color = m_textColor;
        text.characterSpacing = 8;
        text.fontStyle = FontStyles.Bold;
        text.horizontalAlignment = HorizontalAlignmentOptions.Center;
        text.verticalAlignment = VerticalAlignmentOptions.Middle;
        text.SetAllDirty();
        text.ForceMeshUpdate();

        SceneManager.MoveGameObjectToScene(m_generatedRoot, m_previewScene);

        UpdateCamera();

        Debug.Log("Mural Image Generated!");
    }

    private void ClearScene()
    {
        if (!m_previewScene.IsValid())
        {
            InitScene();
        }

        if (m_generatedRoot != null)
        {
            DestroyImmediate(m_generatedRoot);
        }
    }

    private float MonteCarloRandom(float bias)
    {
        float r = (float)rng.NextDouble();
        return Mathf.Pow(r, bias);
    }

    private Material SetMaterial(Material mat, Color color)
    {
        Material copy = new Material(mat);
        copy.SetColor("_Color", color);
        copy.SetVector("_NoiseOffset", new Vector2((MonteCarloRandom(1.0f) - 0.5f) * 12.0f, (MonteCarloRandom(1.0f) - 0.5f) * 12.0f));
        copy.SetFloat("_NoiseStrength", 0.355f);
        copy.SetFloat("_EdgeSharpness", m_splashEdgeSharpness);
        copy.SetFloat("_Size", m_splashFillScale);
        return copy;
    }

    Vector3 GetRandomPointOnCircle(Vector3 center, float radius)
    {
        float minRadius = 0.05f;
        float angle = MonteCarloRandom(1.0f + m_circleAngleRandomScaler) * 2.0f * Mathf.PI;
        float distance = MonteCarloRandom(1.0f + m_circleDistanceRandomScaler) * (radius - minRadius) + minRadius;
        return center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * distance;
    }

    private GameObject CreateSplash(Transform parent, Vector3 position, Vector3 scale, Color color, string name = "Splash")
    {
        GameObject splash = Instantiate(m_splashPrefab, position, Quaternion.Euler(0.0f, 0.0f, 0.0f), parent);
        splash.name = name;
        splash.transform.localScale = scale;

        SpriteRenderer sr = splash.GetComponent<SpriteRenderer>();
        //sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        sr.material = SetMaterial(sr.sharedMaterial, color);

        SplashLinesGenerator sl = splash.GetComponent<SplashLinesGenerator>();
        sl.SetColor(color);
        sl.GenerateLines(color.a * 0.9f, false);

        return splash;
    }

    private void LoadData()
    {
        string path = EditorUtility.OpenFilePanel("Load Settings", "Assets", "asset");

        if (!string.IsNullOrEmpty(path))
        {
            // Convert absolute path to relative project path
            string relativePath = "Assets" + path.Substring(Application.dataPath.Length);

            MuralImageGeneratorSettings loadedData = AssetDatabase.LoadAssetAtPath<MuralImageGeneratorSettings>(relativePath);

            if (loadedData != null)
            {
                m_text = loadedData.text;
                m_font = loadedData.font;
                m_textSize = loadedData.textSize;
                m_textColor = loadedData.textColor;
                m_backgroundRadius = loadedData.backgroundRadius;
                m_backgroundColor = loadedData.backgroundColor;
                m_backgroundBorder = loadedData.backgroundBorder;
                m_numberOfSplashes = loadedData.numberOfSplashes;
                m_splashPrefab = loadedData.splashPrefab;
                m_splashAlpha = loadedData.splashAlpha;
                m_splashEdgeSharpness = loadedData.splashEdgeSharpness;
                m_splashScale = loadedData.splashScale;
                m_splashFillScale = loadedData.splashFillScale;
                m_circleAngleRandomScaler = loadedData.circleAngleRandomScaler;
                m_circleDistanceRandomScaler = loadedData.circleDistanceRandomScaler;

                m_materials.Clear();
                RefreshDropdown(m_font, loadedData.presetMaterial);

                m_colors.Clear();
                m_colors.AddRange(loadedData.colorList);
                m_colorsList.RefreshItems();

                Repaint();

                Debug.Log("Settings loaded succesfully: " + relativePath);
            }
            else
            {
                EditorUtility.DisplayDialog("Load Failed", "Could not load settings data from file.", "OK");
            }
        }
    }

    private void SaveData()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Settings", "NewSettingsData", "asset", "Please enter a file name to save the settings to");

        if (!string.IsNullOrEmpty(path))
        {
            MuralImageGeneratorSettings asset = ScriptableObject.CreateInstance<MuralImageGeneratorSettings>();

            asset.text = m_text;
            asset.font = m_font;
            asset.presetMaterial = m_presetMaterial;
            asset.textSize = m_textSize;
            asset.textColor = m_textColor;
            asset.backgroundRadius = m_backgroundRadius;
            asset.backgroundColor = m_backgroundColor;
            asset.backgroundBorder = m_backgroundBorder;
            asset.numberOfSplashes = m_numberOfSplashes;
            asset.splashPrefab = m_splashPrefab;
            asset.splashAlpha = m_splashAlpha;
            asset.splashEdgeSharpness = m_splashEdgeSharpness;
            asset.splashScale = m_splashScale;
            asset.splashFillScale = m_splashFillScale;
            asset.circleAngleRandomScaler = m_circleAngleRandomScaler;
            asset.circleDistanceRandomScaler = m_circleDistanceRandomScaler;

            asset.colorList = new List<Color>();
            asset.colorList.AddRange(m_colors);

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
