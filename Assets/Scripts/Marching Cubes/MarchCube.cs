using System.Collections.Generic;
using UnityEngine;

public class MarchCube : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int TerrainSize = 128;

    [Header("Texture Settings")]
    public int textureWidth = 256;
    public int textureHeight = 256;
    public int textureDepth = 256;
    public Vector3 Offset = Vector3.zero;
    public float NoiseScale = 2f;

    [Header("Exibition Settings")]
    public float RotateSpeed = 10f;
    public Material material;

    MarchingCube mc = new MarchingCube();
    List<Vector3> vertexList = new List<Vector3>();
    List<int> indexList = new List<int>();

    private void Start() {
        generateNewTerrain();
    }

    public void generateNewTerrain()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        //Calculate density values
        mc.densityValues = calcualteNoiseValues3D();
        watch.Stop();
        Debug.Log("Density generation time: " + watch.ElapsedMilliseconds + " ms");

        watch.Reset();
        watch.Start();
        //For each point in terrain, except last, generate triangles
        for (int i = 0; i < TerrainSize - 1; i++)
        {
            for (int j = 0; j < TerrainSize - 1; j++)
            {
                for (int k = 0; k < TerrainSize - 1; k++)
                {
                    mc.march(i, j, k, vertexList, indexList);
                }
            }
        }
        Debug.Log("Number of vertex: " + vertexList.Count);
        //Split meshes and add it to game
        SplitMeshes();
        Camera.main.transform.position = new Vector3(0, 0, -TerrainSize - (TerrainSize/2));
        watch.Stop();
        Debug.Log("Generation time: " + watch.ElapsedMilliseconds + " ms");
    }

    //Rotates
    private void FixedUpdate()
    {
        this.transform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime);
    }
    
    //Calculate density values based on a Perlin Noise 3D
    float[,,] calcualteNoiseValues3D()
    {
        float[,,] _returnValues = new float[TerrainSize, TerrainSize, TerrainSize];

        NoiseTexture.NoiseScale = NoiseScale;
        NoiseTexture.Offset = Offset;

        Texture3D noiseTexture = NoiseTexture.generateTexture3D_GPU(textureWidth, textureHeight, textureDepth);

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

    //Since the max number of vertices in a mesh should by 65000, this code splits it in meshes with maxVerticesPerMesh (30000) vertices
    void SplitMeshes()
    {
        int maxVerticesPerMesh = 30000;
        int numMeshes = vertexList.Count / maxVerticesPerMesh + 1;

        for (int i = 0; i < numMeshes; i++)
        {
            List<Vector3> splitVertex = new List<Vector3>();
            List<int> splitIndex = new List<int>();

            for (int j = 0; j < maxVerticesPerMesh; j++)
            {
                int index = i * maxVerticesPerMesh + j;
                if (index < vertexList.Count)
                {
                    splitVertex.Add(vertexList[index]);
                    splitIndex.Add(j);
                }
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(splitVertex);
            mesh.SetTriangles(splitIndex, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            GameObject go = new GameObject("Mesh" + i);
            go.transform.parent = this.transform;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().material = material;
            go.GetComponent<MeshFilter>().mesh = mesh;

            go.transform.localPosition = new Vector3(-TerrainSize/2, -TerrainSize/2, -TerrainSize/2);
        }
    }

}
