using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Threading;

[CustomEditor(typeof(MarchCube))]
public class MarchingCubeEditor : Editor
{
    MarchCube mc;

    void OnEnable()
    {
        mc = (MarchCube)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate New Mesh"))
        {
            mc.generateNewTerrain();
        }
    }
}
