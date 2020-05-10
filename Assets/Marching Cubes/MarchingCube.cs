using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCube : MonoBehaviour
{
    public int StepSize = 1;
    public int TerrainSize = 20;

    [Header("Noise settings")]
    public int textureWidth = 256;
    public int textureHeight = 256;
    public int textureDepth = 256;
    [Range(0.00f, 1.00f)]
    public float isosurfaceValue = 0.485f;

    [Header("Generation Settings")]
    public bool autoUpdate = true;

    public float[,,] densityValues;

    private Texture3D noiseTexture;

    private Vector3[] points = new Vector3[8];

    public MarchingCube(float[,,] array)
    {
        densityValues = array;
    }

    int getCaseValue(int x, int y, int z)
    {
        int _returnValue = 0;
        for (int _x = 0; _x <= 1; _x++)
        {
            for (int _y = 0; _y <= 1; _y++)
            {
                for (int _z = 0; _z <= 1; _z++)
                {
                    _returnValue |= ((densityValues[x + _x, y + _y, z + _z] > isosurfaceValue ? 1 : 0) << (_x * 4 + _y * 2 + _z));
                }
            }
        }
        return _returnValue;
    }

    Vector3 vertexInterpolation(Vector3 p1, Vector3 p2, int vp1, int vp2)
    {
        float t = (isosurfaceValue - vp1) / (vp2 - vp1);
        float _x = p1.x + t * (p2.x - p1.x);
        float _y = p1.y + t * (p2.y - p1.y);
        float _z = p1.z + t * (p2.z - p1.z);
        return new Vector3(_x, _y, _z);
    }

    public float[,,] calcualteNoiseValues(bool newNoiseTexture = true)
    {
        float[,,] _returnValues = new float[TerrainSize, TerrainSize, TerrainSize];
        if (newNoiseTexture)
            noiseTexture = NoiseTexture.generateTexture3D(textureWidth, textureHeight, textureDepth);

        float gridStepSizeX = textureWidth / TerrainSize;
        float gridStepSizeY = textureHeight / TerrainSize;
        float gridStepSizeZ = textureDepth / TerrainSize;

        for (int _x = 0; _x < TerrainSize; _x++)
        {
            for (int _y = 0; _y < TerrainSize; _y++)
            {
                for (int _z = 0; _z < TerrainSize; _z++)
                {
                    _returnValues[_x, _y, _z] = noiseTexture.GetPixel((int)(_x * gridStepSizeX), (int)(_y * gridStepSizeY), (int)(_z * gridStepSizeZ)).grayscale;
                }
            }
        }
        return _returnValues;
    }

    public Mesh generateMesh()
    {
        Mesh _returnMesh = new Mesh();
        densityValues = calcualteNoiseValues();
        List<Vector3> _vertices = new List<Vector3>();

        for (int _x = 0; _x < TerrainSize - 1; _x++)
        {
            for (int _y = 0; _y < TerrainSize - 1; _y++)
            {
                for (int _z = 0; _z < TerrainSize - 1; _z++)
                {
                    int cubeIndex = getCaseValue(_x, _y, _z);
                    Vector3 p0 = new Vector3(_x, _y, _z);
                    //if (cubeCase[cubeIndex] & 1 != 0) _vertices.Add(vertexInterpolation(p0, ))
                }
            }
        }

        return _returnMesh;
    }
    private void OnDrawGizmosSelected()
    {
        if (densityValues != null)
        {
            for (int _x = 0; _x < TerrainSize; _x++)
            {
                for (int _y = 0; _y < TerrainSize; _y++)
                {
                    for (int _z = 0; _z < TerrainSize; _z++)
                    {
                        Gizmos.color = new Color(densityValues[_x, _y, _z], densityValues[_x, _y, _z], densityValues[_x, _y, _z]);
                        Gizmos.DrawSphere(new Vector3(_x, _y, _z), 0.1f);
                    }
                }
            }
        }
    }
}