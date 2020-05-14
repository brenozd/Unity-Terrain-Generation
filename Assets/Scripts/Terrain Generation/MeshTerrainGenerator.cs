using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTerrainGenerator : MonoBehaviour
{
    public int size = 128;
    public Material material;
    Vector3[] vertex;
    int[] triangles;


    private void Start()
    {
        generateMesh();
    }
    void generateMesh()
    {
        vertex = new Vector3[(size + 1)*(size+1)];
        for (int i = 0, x = 0; x <= size; x++)
        {
            for (int z = 0; z <= size; z++, i++)
            {
                float y = Noise.PerlinNoise2D(x *.15f, z*.15f) * 2f;
                vertex[i] = (new Vector3(x, y, z));
            }
        }

        triangles = new int[size * size * 6];

        int ti = 0;
        int vi = 0;
        for (int y = 0; y < size; y++, vi++) {
			for (int x = 0; x < size; x++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + size + 1;
				triangles[ti + 5] = vi + size + 2;
                ti+=6;
                vi++;
			}
		}

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertex);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();
        //Normals are inverted
        mesh.RecalculateNormals();

        GameObject go = new GameObject("Mesh");
        go.transform.parent = this.transform;
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<Renderer>().material = material;
        go.GetComponent<MeshFilter>().mesh = mesh;

        go.transform.localPosition = new Vector3(-size / 2, -size / 2, -size / 2);
    }
}
