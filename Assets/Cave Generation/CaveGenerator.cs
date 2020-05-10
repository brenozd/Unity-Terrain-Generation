using UnityEngine;
using System.Collections.Generic;
public class CaveGenerator : MonoBehaviour
{
    [Header("Cave settings")]
    public int caveWidth = 50;
    public int caveHeight = 50;

    [Header("Noise settings")]
    public int textureWidth = 256;
    public int textureHeight = 256;

    [Range(0.000f, 1.000f)]
    public float offset = 0.495f;
    public float noiseScale = 6f;
    public int Seed = 0;

    [Header("CA Map settings")]
    public int nearbyWallsToFloor = 2;
    public int nearbyWallsToWall = 5;
    public int nearbyWallsToMantain = 4;

    [Header("Prefab settings")]
    public GameObject prefab;

    [Header("Generation Settings")]
    public bool autoUpdate = true;

    float[,] values;
    GameObject parent;

    void Awake()
    {
        Noise.Seed = Seed;
        if (GameObject.Find("Cells") == null)
            parent = new GameObject("Cells");
        else
            parent = GameObject.Find("Cells");
        generateCave();
    }

    public void generateCave()
    {
        if (parent != null)
        {
            values = new float[caveWidth, caveHeight];
            values = caMap(generateNoiseValues());
            printGame();
        }

    }

    int computeNeighbors(int x, int y, float[,] array)
    {
        int neighbors = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int _x = x + i > caveWidth - 1 ? 0 : x + i < 0 ? caveWidth - 1 : x + i;
                int _y = y + j > caveHeight - 1 ? 0 : y + j < 0 ? caveHeight - 1 : y + j;
                neighbors += array[_x, _y] >= offset ? 1 : 0;
            }
        }
        neighbors -= array[x, y] >= offset ? 1 : 0;
        return neighbors;
    }

    float[,] generateNoiseValues()
    {
        NoiseTexture.Seed = Seed;
        NoiseTexture.NoiseScale = noiseScale;

        NoiseTexture.clearPermutationTable();
        Texture2D noiseTexture = NoiseTexture.generateTexture2D(textureWidth, textureHeight);
        float[,] _returnList = new float[caveWidth, caveHeight];

        float gridStepSizeX = textureWidth / caveWidth;
        float gridStepSizeY = textureHeight / caveHeight;


        for (int x = 0; x < caveWidth; x++)
        {
            for (int y = 0; y < caveHeight; y++)
            {
                _returnList[x, y] = noiseTexture.GetPixel((int)(x * gridStepSizeX), (int)(y * gridStepSizeY)).grayscale;
            }
        }
        return _returnList;
    }

    float[,] caMap(float[,] array)
    {
        float[,] _returnList = new float[caveWidth, caveHeight];
        for (int x = 0; x < caveWidth; x++)
        {
            for (int y = 0; y < caveHeight; y++)
            {
                if (computeNeighbors(x, y, array) >= nearbyWallsToWall && array[x, y] < offset)
                    _returnList[x, y] = 1.0f;
                else if (computeNeighbors(x, y, array) >= nearbyWallsToMantain && array[x, y] >= offset)
                    _returnList[x, y] = 1.0f;
                else if (computeNeighbors(x, y, array) <= nearbyWallsToFloor && array[x, y] >= offset)
                    _returnList[x, y] = 0.0f;
                else _returnList[x, y] = array[x, y];
            }
        }
        return _returnList;
    }

    public void printGame()
    {
        foreach (Transform go in parent.GetComponentInChildren<Transform>())
        {
            Destroy(go.gameObject);
        }

        for (int i = 0; i < caveWidth; i++)
        {
            for (int j = 0; j < caveHeight; j++)
            {
                GameObject go = Instantiate(prefab, new Vector3(i, j, 0), Quaternion.identity, parent.transform);
                if (values[i, j] < offset)
                    go.GetComponent<Renderer>().material.color = Color.black;
                else
                    go.GetComponent<Renderer>().material.color = Color.white;
            }
        }
    }
}
