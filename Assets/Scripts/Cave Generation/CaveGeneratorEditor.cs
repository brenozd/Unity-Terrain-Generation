using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Threading;

[CustomEditor(typeof(CaveGenerator))]
public class CaveGeneratorEditor : Editor
{
    CaveGenerator caveGen = null;

    void OnEnable()
    {
        caveGen = (CaveGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        if (caveGen != null)
        {
            if (DrawDefaultInspector() && caveGen.autoUpdate)
            {
                var watch = new Stopwatch();
                watch.Start();
                caveGen.generateCave();
                watch.Stop();
                UnityEngine.Debug.LogWarning("Runtime: " + watch.ElapsedMilliseconds);
            }
            if (!caveGen.autoUpdate && GUILayout.Button("Generate new cave"))
            {
                var watch = new Stopwatch();
                watch.Start();
                caveGen.generateCave();
                watch.Stop();
                UnityEngine.Debug.LogWarning("Runtime: " + watch.ElapsedMilliseconds);
            }
        }

    }
}
