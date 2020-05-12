using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Threading;

[CustomEditor(typeof(MarchingCube))]
public class MarchingCubeEditor : Editor
{
    MarchingCube mc;

    void OnEnable()
    {
        mc = (MarchingCube)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate New Mesh"))
        {
            //mc.generateMesh();
        }
    }
}
