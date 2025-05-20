using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MuralImageGeneratorEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    // Bindings
    [SerializeField]
    private Texture2D m_previewTexture;
    [SerializeField]
    private Sprite m_circleSprite;
    [SerializeField]
    private Sprite m_splashSprite;
    [SerializeField]
    private Material m_splashMaterial;
    [SerializeField]
    private string m_text;
    [SerializeField]
    private TMP_FontAsset m_font;
    [SerializeField]
    private int m_numberOfSplashes;
    [SerializeField]
    private Color m_color_1;
    [SerializeField]
    private Color m_color_2;
    [SerializeField]
    private Color m_color_3;

    // Preview
    private int m_previewSize = 256;
    private Scene m_previewScene;
    private GameObject m_cameraObject;
    private Camera m_sceneCamera;

    // Buttons
    private Button m_generateButton;
    private Button m_saveButton;
    private Button m_loadSetButton;
    private Button m_saveSetButton;

    // Generated Root
    private GameObject m_generatedRoot;

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

        rootVisualElement.Q<VisualElement>("Container").dataSource = this;

        // Get Elements
        m_saveButton = rootVisualElement.Q<Button>("SaveImage");
        m_saveButton.clicked += Export;
        m_generateButton = rootVisualElement.Q<Button>("Generate");
        m_generateButton.clicked += Generate;

        m_loadSetButton = rootVisualElement.Q<Button>("LoadSettings");
        m_loadSetButton.clicked += LoadData;
        m_saveSetButton = rootVisualElement.Q<Button>("SaveSettings");
        m_saveSetButton.clicked += SaveData;

        InitScene();
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

            m_sceneCamera = m_cameraObject.AddComponent<Camera>();
            m_sceneCamera.aspect = 1;
            m_sceneCamera.backgroundColor = Color.black;
            m_sceneCamera.clearFlags = CameraClearFlags.SolidColor;
            m_sceneCamera.targetTexture = new RenderTexture(m_previewSize, m_previewSize, 32, RenderTextureFormat.ARGBFloat);

            SceneManager.MoveGameObjectToScene(m_cameraObject, m_previewScene);

            m_sceneCamera.scene = m_previewScene;
        }

        UpdateCamera();
    }

    private void UpdateCamera()
    {
        m_sceneCamera.Render();

        if (m_previewTexture == null)
        {
            m_previewTexture = new Texture2D(m_previewSize, m_previewSize, TextureFormat.RGBAFloat, false, true);
        }

        RenderTexture.active = m_sceneCamera.targetTexture;

        m_previewTexture.ReadPixels(new Rect(0, 0, m_previewSize, m_previewSize), 0, 0);
        m_previewTexture.Apply();

        RenderTexture.active = null;
    }

    private void Export()
    {
        m_sceneCamera.depthTextureMode = DepthTextureMode.Depth;
        m_sceneCamera.backgroundColor = new Color(0, 0, 0, 0);

        UpdateCamera();
        string name = "MuralGenerated_" + m_text + "_" + m_font.name + "_" + m_numberOfSplashes.ToString();
        SaveTextureAsPNG(m_previewTexture, name);

        m_sceneCamera.backgroundColor = Color.black;

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
    }

    private void Generate()
    {
        ClearScene();

        // TODO: Code to generate circle with mask and some splashes

        UpdateCamera();
    }

    private void LoadData()
    {
        // TODO: Create ScriptableObject with Data to load
        // TODO: Create Loading of data
        // TODO: Create Saving Data to scriptable object
    }

    private void SaveData()
    {
        // TODO: Create
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
}
