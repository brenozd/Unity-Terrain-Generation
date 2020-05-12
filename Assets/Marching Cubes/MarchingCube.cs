using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCube : MonoBehaviour
{
    ///<summary>
    ///Isosurface Value
    ///Points if density value equals or below this will be considered inactive
    ///</summary>
    public float isosurfaceValue = 0.485f;

    public bool autoUpdate = true;

    public float[,,] densityValues;

    private int index = 0;

    public MarchingCube() { }

    ///<summary> Performs a single march on a point
    ///<param name = "X">
    ///X component of point
    ///</param>
    ///<param name = "Y">
    ///Y component of point
    ///</param>
    ///<param name = "Z">
    ///Z component of point
    ///</param>
    ///<param name = "VertexList">
    ///List of triangle vertex generated from a single (X, Y, Z) point
    ///</param>
    ///<param name = "IndexList">
    ///List of triangle indexes generated from a single (X, Y, Z) point
    ///</param>
    ///</summary>
    public void march(float x, float y, float z, List<Vector3> vertexList, List<int> indexList)
    {
        //Array that stores all 12 possible edge points
        Vector3[] points = new Vector3[12];

        //Flag that indicates how many points will be generated
        int edgeFlag = 0;

        //Get case value of point and it 8 neighbors
        int auxIndex = 0;
        for (int i = 0; i < 8; i++)
            if (densityValues[(int)x + Tables.VertexOffset[i, 0],
                              (int)y + Tables.VertexOffset[i, 1],
                              (int)z + Tables.VertexOffset[i, 2]] <= isosurfaceValue) auxIndex |= 1 << i;

        edgeFlag = Tables.cases[auxIndex];

        //If theres no points return
        if (edgeFlag == 0) return;

        //For every points in edgeFlag (every bit set to 1)
        for (int i = 0; i < 12; i++)
        {
            if ((edgeFlag & (1 << i)) != 0)
            {
                //Get points that match the first element of EdgeConnection table
                Vector3 p1 = new Vector3(x + Tables.VertexOffset[Tables.EdgeConnection[i, 0], 0],
                                         y + Tables.VertexOffset[Tables.EdgeConnection[i, 0], 1],
                                         z + Tables.VertexOffset[Tables.EdgeConnection[i, 0], 2]);

                //Get points that match the second element of EdgeConnection table
                Vector3 p2 = new Vector3(x + Tables.VertexOffset[Tables.EdgeConnection[i, 1], 0],
                                         y + Tables.VertexOffset[Tables.EdgeConnection[i, 1], 1],
                                         z + Tables.VertexOffset[Tables.EdgeConnection[i, 1], 2]);

                //Calculate de interpolation factor
                float t = (isosurfaceValue - densityValues[(int)p1.x, (int)p1.y, (int)p1.z]) / 
                          (densityValues[(int)p2.x, (int)p2.y, (int)p2.z] - densityValues[(int)p1.x, (int)p1.y, (int)p1.z]);

                //Interpolate
                points[i] = Vector3.Lerp(p1, p2, t);
            }
        }

        //For each possible triangle configuration
        for (int i = 0; i < 5; i++)
        {   
            //If theres an tiangle to be added
            if (Tables.triangulationTable[auxIndex, 3 * i] < 0) break;
            //Get triangle index
            index = vertexList.Count;
            //For each triangle points
            for (int j = 0; j < 3; j++)
            {
                //Add vertex triangle to list
                vertexList.Add(points[Tables.triangulationTable[auxIndex, 3 * i + j]]);
                //Add triangle vertex indexes to List
                indexList.Add(index + j);
            }
        }
    }
}