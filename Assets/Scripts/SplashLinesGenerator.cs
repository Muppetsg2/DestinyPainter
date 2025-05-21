using System.Collections.Generic;
using SaintsField;
using UnityEngine;

// TODO: Create Mask based on sprite wchich will affect lines (only visible outside mask)

public class SplashLinesGenerator : MonoBehaviour
{
    [Layer]
    public string renderLayer;
    [Layer]
    public string normalLayer;

    public Camera renderCamera;
    public SpriteRenderer spriteRenderer;
    public GameObject linePrefab;

    public int linesPointsSpacingBoxSize = 5;
    public int linePointCheckBoxSize = 3;
    public float minLinesXDistance = 0.5f;
    public MinMaxValue<int> linesNum = new() { Min = 3, Max = 5 };

    private ColorType colorType;

    private readonly List<GameObject> lines = new();

    private void OnValidate()
    {
        if (linePointCheckBoxSize > linesPointsSpacingBoxSize) linePointCheckBoxSize = linesPointsSpacingBoxSize;
    }

    void Start()
    {
        //GenerateLines();
    }

    public void SetColor(ColorType color)
    {
        colorType = color;
    }

    [ContextMenu("Generate Lines")]
    public void GenerateLines()
    {
        ClearLines();

        // set game object
        gameObject.layer = LayerMask.NameToLayer(renderLayer);

        // Create temp variables
        Sprite sprite = spriteRenderer.sprite;
        RenderTexture renderTexture = new(sprite.texture.width, sprite.texture.height, 0, RenderTextureFormat.ARGB32);
        Texture2D texture = new(sprite.texture.width, sprite.texture.height, TextureFormat.RGBA32, false);

        // Render Camera view
        renderCamera.gameObject.SetActive(true);
        renderCamera.targetTexture = renderTexture;
        renderCamera.Render();

        // reset game object
        gameObject.layer = LayerMask.NameToLayer(normalLayer);

        // destroy camera
        //Destroy(renderCamera.gameObject);
        renderCamera.gameObject.SetActive(false);

        // Copy to texure
        RenderTexture last = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = last;

        // Analize texture
        List<Vector3> avilablePositions = new();
        float unitPerPixel = 1f / sprite.pixelsPerUnit;
        for (int x = 0; x < texture.width; x += linesPointsSpacingBoxSize)
        {
            int sx = x + (linesPointsSpacingBoxSize - linePointCheckBoxSize) / 2;
            if (sx >= texture.width) sx = texture.width - 1;

            for (int y = 0; y < texture.height; y += linesPointsSpacingBoxSize)
            {
                int sy = y + (linesPointsSpacingBoxSize - linePointCheckBoxSize) / 2;
                if (sy >= texture.height) sy = texture.height - 1;

                if (CheckPixelsBox(sx, sy, texture))
                {
                    int cx = x + linesPointsSpacingBoxSize / 2;
                    int cy = y + linesPointsSpacingBoxSize / 2;

                    float posX = (cx - texture.width * 0.5f) * unitPerPixel;
                    float posY = (cy - texture.height * 0.5f) * unitPerPixel;
                    avilablePositions.Add(new Vector3(posX, posY, 0f));
                }
            }
        }

        int pointsToChoose = Random.Range(linesNum.Min, linesNum.Max + 1);

        for (int i = 0; i < pointsToChoose && avilablePositions.Count > 0; ++i)
        {
            int idx = Random.Range(0, avilablePositions.Count);
            Vector3 pos = avilablePositions[idx];
            avilablePositions.RemoveAt(idx);

            for (int j = 0; j < avilablePositions.Count; ++j)
            {
                Vector3 otherPos = avilablePositions[j];
                if (Mathf.Abs(otherPos.x - pos.x) <= minLinesXDistance)
                {
                    avilablePositions.RemoveAt(j);
                    --j;
                }
            }

            GameObject line = Instantiate(linePrefab, transform.position, Quaternion.identity, transform);
            line.transform.position = transform.position + pos;
            var splashLineAnim = line.GetComponent<SplashLineAnimation>();
            splashLineAnim.SetColor(colorType);
            lines.Add(line);
        }

        //for (int i = 0; i < avilablePositions.Count; ++i)
        //{
        //    Vector3 pos = avilablePositions[i];
        //    avilablePositions.RemoveAt(i);

        //    for (int j = 0; j < avilablePositions.Count; ++j)
        //    {
        //        Vector3 otherPos = avilablePositions[j];
        //        if (Mathf.Abs(otherPos.x - pos.x) <= minLinesXDistance)
        //        {
        //            avilablePositions.RemoveAt(j);
        //            --j;
        //        }
        //    }

        //    GameObject line = Instantiate(linePrefab, transform.position, Quaternion.identity, transform);
        //    line.transform.localPosition = pos;
        //    lines.Add(line);
        //}
    }

    [ContextMenu("Clear Lines")]
    void ClearLines()
    {
        foreach (var line in lines)
        {
            Destroy(line);
        }
        lines.Clear();
    }

    bool CheckPixelsBox(int startX, int startY, Texture2D texture)
    {
        for (int x = startX; x < texture.width && x - startX < linePointCheckBoxSize; ++x)
        {
            for (int y = startY; y < texture.height && y - startY < linePointCheckBoxSize; ++y)
            {
                if (texture.GetPixel(x, y).a != 1f)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (spriteRenderer == null) return;

        Sprite sprite = spriteRenderer.sprite;
        if (sprite == null) return;

        Texture2D texture = sprite.texture;

        float unitPerPixel = 1f / sprite.pixelsPerUnit;
        for (int x = 0; x < texture.width; x += linesPointsSpacingBoxSize)
        {
            int sx = x + linesPointsSpacingBoxSize / 2;
            if (sx >= texture.width) sx = texture.width - 1;

            for (int y = 0; y < texture.height; y += linesPointsSpacingBoxSize)
            {
                int sy = y + linesPointsSpacingBoxSize / 2;
                if (sy >= texture.height) sy = texture.height - 1;

                float posX = (sx - texture.width * 0.5f) * unitPerPixel;
                float posY = (sy - texture.height * 0.5f) * unitPerPixel;
                Vector3 pos = transform.TransformPoint(new Vector3(posX, posY, 0));

                // draw pixel boxes
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(pos, transform.TransformVector(linesPointsSpacingBoxSize * unitPerPixel * Vector3.one));

                // draw check boxes
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(pos, transform.TransformVector(linePointCheckBoxSize * unitPerPixel * Vector3.one));
            }
        }
    }
}
