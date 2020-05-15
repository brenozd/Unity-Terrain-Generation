using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTerrainGenerator : MonoBehaviour
{
    public int size = 512;
    public Material material;
    struct MeshComponents
    {
        public Vector3[] vertices;
        public int[] triangles;
    }


    private void Start()
    {
        Mesh[] meshes = SplitMeshes(CalculateMesh(size), size);
        foreach (Mesh m in meshes)
        {
            m.RecalculateNormals();
            m.RecalculateBounds();

            GameObject go = new GameObject("Mesh");
            go.transform.parent = this.transform;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().material = material;
            go.GetComponent<MeshFilter>().mesh = m;

            //go.transform.localPosition = new Vector3(-size / 2, -size / 2, -size / 2);
        }
    }

    MeshComponents CalculateMesh(int size)
    {
        MeshComponents mesh = new MeshComponents();
        mesh.vertices = new Vector3[(size + 1) * (size + 1)];
        mesh.triangles = new int[size * size * 6];

        for (int i = 0, x = 0; x <= size; x++)
        {
            for (int z = 0; z <= size; z++, i++)
            {
                float y = Noise.PerlinNoise2D(x * 0.05f, z * 0.05f) * 8;
                mesh.vertices[i] = (new Vector3(x, y, z));
            }
        }

        return mesh;
    }

    Mesh[] SplitMeshes(MeshComponents mesh, int size)
    {
        //vertsPerMesh > 2*(size+1)
        //vertsPerMesh < 64512 (65534 exatctly)
        //vertsPerMesh must be multiple of 2
        
        int aux = 0;
        int powerBase = 2;
        while (aux * size+1 < 30000){
            powerBase++;
            aux = (int)Mathf.Pow(2, powerBase);
        }


        int vertsPerMesh = (size + 1) * (int)Mathf.Pow(2.0f, powerBase);
        Debug.Log(vertsPerMesh);
        int numMeshes = mesh.vertices.Length / vertsPerMesh + 1;
        Mesh[] returnMeshes = new Mesh[numMeshes];

        for (int m = 0; m < numMeshes; m++)
        {
            returnMeshes[m] = new Mesh();
            List<Vector3> mVertices = new List<Vector3>();
            List<int> mTriangles = new List<int>();

            int index = 0;
            Debug.LogWarning(mesh.vertices.Length);
            for (int v = 0; v < vertsPerMesh; v++)
            {
                index = (m * (vertsPerMesh - (size + 1)) + v);
                if (index < mesh.vertices.Length) 
                    mVertices.Add(mesh.vertices[index]);
                //Debug.Log("M: " + m + " vertsPerMesh: " + vertsPerMesh + " size+1: " + (size+1) + " v: " + v + " index: " + index);
            }

            for (int v = 0, x = 0; x < (mVertices.Count / (size + 1)) - 1; x++)
            {
                for (int z = 0; z < size; z++, v++)
                {
                    mTriangles.Add(v);
                    mTriangles.Add(v + 1);
                    mTriangles.Add(v + size + 1);
                    mTriangles.Add(v + size + 1);
                    mTriangles.Add(v + 1);
                    mTriangles.Add(v + size + 2);
                }
                v++;
            }
            returnMeshes[m].SetVertices(mVertices);
            returnMeshes[m].SetTriangles(mTriangles, 0);
        }

        return returnMeshes;
    }
}
