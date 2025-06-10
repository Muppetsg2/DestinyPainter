using SaintsField.Playa;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextParticleSystemController : MonoBehaviour
{
    public TextMeshPro sourceText;
    public ParticleSystem textParticleSystem;
    private ParticleSystemRenderer rendererSystem;

    private List<Mesh> characterMeshes = new List<Mesh>();
    private int maxMeshes = 4;

    private void OnValidate()
    {
        UpdateParticleSystemMeshes();
    }

    void Start()
    {
        UpdateParticleSystemMeshes();
        UpdateParticleSystemValues();
    }

    public void UpdateParticleSystemValues()
    {
        if (textParticleSystem == null) return;

        float mult = Mathf.Min(transform.lossyScale.x, transform.lossyScale.y);

        var mainModule = textParticleSystem.main;

        mainModule.startSpeed = new ParticleSystem.MinMaxCurve(mainModule.startSpeed.constant * mult);
        mainModule.startSize = new ParticleSystem.MinMaxCurve(mainModule.startSize.constantMin * mult, mainModule.startSize.constantMax * mult);

        var shapeModule = textParticleSystem.shape;

        shapeModule.radius *= mult;
        shapeModule.radiusThickness *= mult;
    }

    [Button]
    public void UpdateParticleSystemMeshes()
    {
        if (sourceText == null || textParticleSystem == null) return;

        sourceText.enabled = true;

        sourceText.ForceMeshUpdate();

        GenerateCharacterMeshes();

        rendererSystem = textParticleSystem.GetComponent<ParticleSystemRenderer>();
        Mesh[] meshes = new Mesh[maxMeshes];
        int count = Mathf.Min(characterMeshes.Count, maxMeshes);
        for (int i = 0; i < count; ++i)
        {
            int meshIndex = Random.Range(0, characterMeshes.Count);
            meshes[i] = characterMeshes[meshIndex];
            characterMeshes.RemoveAt(meshIndex);
        }

        rendererSystem.SetMeshes(meshes);

        sourceText.enabled = false;
    }

    private void GenerateCharacterMeshes()
    {
        TMP_TextInfo textInfo = sourceText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // Jeœli to spacja lub znak kontrolny, pomiñ
            if (!charInfo.isVisible)
            {
                continue;
            }

            // Generuj siatkê dla pojedynczej litery
            Mesh charMesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            // Pobierz indeksy wierzcho³ków dla bie¿¹cej litery
            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // SprawdŸ, czy indeksy s¹ prawid³owe
            if (materialIndex < textInfo.meshInfo.Length && vertexIndex + 3 < textInfo.meshInfo[materialIndex].vertices.Length)
            {
                // Pobierz wierzcho³ki
                Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;
                Vector4[] sourceUVs = textInfo.meshInfo[materialIndex].uvs0;

                // Oblicz œrodek litery
                // TMP_CharacterInfo.bottomLeft i .topRight daj¹ nam bounding box litery
                Vector3 charCenter = (charInfo.bottomLeft + charInfo.topRight) / 2f;

                // Dodaj wierzcho³ki, UV i trójk¹ty do tymczasowych list dla tej litery
                // Przesuniêcie do œrodka litery poprzez odjêcie jej œrodka
                vertices.Add(sourceVertices[vertexIndex + 0] - charCenter);
                vertices.Add(sourceVertices[vertexIndex + 1] - charCenter);
                vertices.Add(sourceVertices[vertexIndex + 2] - charCenter);
                vertices.Add(sourceVertices[vertexIndex + 3] - charCenter);

                uvs.Add(sourceUVs[vertexIndex + 0]);
                uvs.Add(sourceUVs[vertexIndex + 1]);
                uvs.Add(sourceUVs[vertexIndex + 2]);
                uvs.Add(sourceUVs[vertexIndex + 3]);

                // Trójk¹ty dla kwadratu (0,1,2,3)
                triangles.Add(0);
                triangles.Add(1);
                triangles.Add(2);

                triangles.Add(2);
                triangles.Add(3);
                triangles.Add(0);

                charMesh.SetVertices(vertices);
                charMesh.SetTriangles(triangles, 0);
                charMesh.SetUVs(0, uvs);
                charMesh.RecalculateNormals(); // Wymagane dla oœwietlenia
                charMesh.RecalculateBounds(); // Wymagane dla poprawnego renderowania

                characterMeshes.Add(charMesh);
            }
        }
    }
}