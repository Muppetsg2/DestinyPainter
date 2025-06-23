using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

public class SplashLinesGenerator : MonoBehaviour
{
    [Layer] public string renderLayer;
    [Layer] public string normalLayer;

    public Camera renderCamera;
    public SpriteRenderer spriteRenderer;
    public SpriteMask mask;
    public GameObject linePrefab;

    public int linesPointsSpacingBoxSize = 5;
    public int linePointCheckBoxSize = 3;
    public float minLinesXDistance = 0.5f;
    public MinMaxValue<int> linesNum = new() { Min = 3, Max = 5 };

    private Color col;

    private readonly List<GameObject> lines = new();

    private void OnValidate()
    {
        if (linePointCheckBoxSize > linesPointsSpacingBoxSize) linePointCheckBoxSize = linesPointsSpacingBoxSize;
    }

    public void SetColor(Color color)
    {
        col = color;
    }

    [ContextMenu("Generate Lines")]
    public void ContextMenuGenerateLines()
    {
        col = Color.white;
        GenerateLines(0.5f, false);
        col = Color.black;
        col.a = 0.0f;
    }

    public void GenerateLines(float alpha, bool anim = true)
    {
        ClearLines();

        // set game object
        gameObject.layer = LayerMask.NameToLayer(renderLayer);

        // Create temp variables
        Sprite sprite = spriteRenderer.sprite;
        RenderTexture renderTexture = new(sprite.texture.width, sprite.texture.height, 0, RenderTextureFormat.ARGB32);
        Texture2D texture = new(sprite.texture.width, sprite.texture.height, TextureFormat.RGBA32, false);

        // Change spriteColor
        Color spriteColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;

        // Render Camera view
        renderCamera.gameObject.SetActive(true);
        renderCamera.orthographicSize = transform.TransformVector(0.5f * Vector3.one).x;
        renderCamera.transform.localPosition = new Vector3(0, 0, renderCamera.orthographicSize != 0.0f ? - (1.0f / renderCamera.orthographicSize) : -1.0f);
        renderCamera.targetTexture = renderTexture;
        renderCamera.Render();

        // reset game object
        gameObject.layer = LayerMask.NameToLayer(normalLayer);

        // destroy camera
        //Destroy(renderCamera.gameObject);
        renderCamera.gameObject.SetActive(false);

        // reset spriteColor
        spriteRenderer.color = spriteColor;

        // Copy to texure
        RenderTexture last = RenderTexture.active;
        RenderTexture.active = renderCamera.targetTexture;
        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = last;

        // Set Mask Sprite
        Sprite maskSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), 0.5f * Vector2.one);
        mask.sprite = maskSprite;
        mask.alphaCutoff = alpha;

        // Analize texture
        Dictionary<float, float> avilablePositions = new();
        float unitPerPixel = 1f / sprite.pixelsPerUnit;
        for (int x = 0; x < texture.width; x += linesPointsSpacingBoxSize)
        {
            int sx = x + (linesPointsSpacingBoxSize - linePointCheckBoxSize) / 2;
            if (sx >= texture.width) sx = texture.width - 1;

            for (int y = 0; y < texture.height; y += linesPointsSpacingBoxSize)
            {
                int sy = y + (linesPointsSpacingBoxSize - linePointCheckBoxSize) / 2;
                if (sy >= texture.height) sy = texture.height - 1;

                if (CheckPixelsBox(sx, sy, texture, alpha))
                {
                    int cx = x + linesPointsSpacingBoxSize / 2;
                    int cy = y + linesPointsSpacingBoxSize / 2;

                    float posX = (cx - texture.width * 0.5f) * unitPerPixel;
                    float posY = (cy - texture.height * 0.5f) * unitPerPixel;

                    Vector3 pos = transform.TransformVector(new Vector3(posX, posY, 0f));

                    if (avilablePositions.ContainsKey(pos.x))
                    {
                        if (avilablePositions[pos.x] <= pos.y)
                        {
                            continue;
                        }
                    }

                    avilablePositions[pos.x] = pos.y;
                }
            }
        }

        int pointsToChoose = Random.Range(linesNum.Min, linesNum.Max + 1);
        for (int i = 0; i < pointsToChoose && avilablePositions.Count > 0; ++i)
        {
            float posX = avilablePositions.Keys.ElementAt(Random.Range(0, avilablePositions.Count));
            float posY = avilablePositions[posX];
            avilablePositions.Remove(posX);

            for (int j = 0; j < avilablePositions.Count; ++j)
            {
                float otherPosX = avilablePositions.Keys.ElementAt(j);
                if (Mathf.Abs(otherPosX - posX) <= minLinesXDistance)
                {
                    avilablePositions.Remove(otherPosX);
                    --j;
                }
            }

            GameObject line = Instantiate(linePrefab, transform.position, Quaternion.identity, transform);
            lines.Add(line);
            line.transform.position += new Vector3(posX, posY, 0f);
            var splashLineAnim = line.GetComponent<SplashLineAnimation>();
#if UNITY_EDITOR
            splashLineAnim.Setup();
#endif
            splashLineAnim.SetColor(col);
            if (anim) splashLineAnim.PlayAnim();
            else splashLineAnim.Complete();
        }

        //for (int i = 0; i < avilablePositions.Count;)
        //{
        //    float posX = avilablePositions.Keys.ElementAt(i);
        //    float posY = avilablePositions[posX];
        //    avilablePositions.Remove(posX);

        //    for (int j = 0; j < avilablePositions.Count; ++j)
        //    {
        //        float otherPosX = avilablePositions.Keys.ElementAt(j);
        //        if (Mathf.Abs(otherPosX - posX) <= minLinesXDistance)
        //        {
        //            avilablePositions.Remove(otherPosX);
        //            --j;
        //        }
        //    }

        //    GameObject line = Instantiate(linePrefab, transform.position, Quaternion.identity, transform);
        //    lines.Add(line);
        //    line.transform.position += new Vector3(posX, posY, 0f);
        //}
    }

    [ContextMenu("Clear Lines")]
    void ClearLines()
    {
        foreach (var line in lines)
        {
            //Destroy(line);
            DestroyImmediate(line);
        }
        lines.Clear();
    }

    bool CheckPixelsBox(int startX, int startY, Texture2D texture, float alpha)
    {
        for (int x = startX; x < texture.width && x - startX < linePointCheckBoxSize; ++x)
        {
            for (int y = startY; y < texture.height && y - startY < linePointCheckBoxSize; ++y)
            {
                if (texture.GetPixel(x, y).a < alpha)
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
