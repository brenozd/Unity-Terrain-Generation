using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("HeightMap Settings")]
    [Range(4, 2048)]
    public int textureWidth;
    [Range(4, 2048)]
    public int textureHeight;
    [Range(1, 20)]
    public float noiseScale;

    [Header("Terrain Settings")]
    [Range(1, 512)]
    public int terrainWidth;
    [Range(1, 512)]
    public int terrainHeight;
    [Range(1, 20)]
    public float visualizationScale;

    [Header("Octave Settings")]
    [Range(0, 8)]
    public int octaves = 8;
    [Range(-10, 10)]
    public float persistence = 0.25f;
    [Range(-20, 20)]
    public float lacunarity = 2;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Real time settings")]
    public bool autoUpdate = true;

    Terrain _terrain;
    float[,] terrainHeights;

    List<GameObject> points = new List<GameObject>();
    GameObject parent;

    public TerrainGenerator() { }

    private void Start()
    {
        if (GameObject.Find("Terrain") == null)
            parent = new GameObject("Terrain");
        else
            parent = GameObject.Find("Terrain");
        generateCubeGrid();
    }
    Texture2D generateTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                texture.SetPixel(x, y, calculateColor(x, y));
            }
        }
        texture.Apply();
        return texture;
    }

    Color calculateColor(float x, float y)
    {
        float xCoord = (float)x / textureWidth * noiseScale;
        float yCoord = (float)y / textureHeight * noiseScale;
        float sample = (float)Noise.PerlinNoise2D(xCoord, yCoord, octaves, persistence, lacunarity);
        return new Color(sample, sample, sample);
    }

    public void generateCubeGrid()
    {
        foreach (GameObject go in points)
        {
            Destroy(go);
        }
        points.Clear();
        Texture2D perlinTexture = generateTexture(textureWidth, textureHeight);
        for (float x = 0; x < terrainWidth; x += 1f)
        {
            for (float z = 0; z < terrainHeight; z += 1f)
            {
                float gridStepSizeY = textureHeight / terrainHeight;
                float gridStepSizeX = textureWidth / terrainWidth;
                points.Add(Instantiate(prefab, new Vector3(x, perlinTexture.GetPixel((int)(x * gridStepSizeX), (int)(z * gridStepSizeY)).grayscale * visualizationScale, z), Quaternion.identity, parent.transform));
            }
        }
    }
}
