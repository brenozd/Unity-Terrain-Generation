using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    TerrainGenerator pnt;
    void OnEnable()
    {
        pnt = (TerrainGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        if (DrawDefaultInspector() && pnt.autoUpdate)
        {
            pnt.generateCubeGrid();
        }
        if (GUILayout.Button("Generate Grid"))
        {
            pnt.generateCubeGrid();
        }
    }
}
