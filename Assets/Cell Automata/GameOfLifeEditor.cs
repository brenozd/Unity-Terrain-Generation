using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Threading;

[CustomEditor(typeof(GameOfLife))]
public class GameOfLifeEditor : Editor
{
    GameOfLife gol;

    void OnEnable()
    {
        gol = (GameOfLife)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Do Step"))
        {
            var watch = new Stopwatch();
            watch.Start();
            gol.doStep(true);
            watch.Stop();
            UnityEngine.Debug.LogWarning("Runtime: " + watch.ElapsedMilliseconds);
        }
        if (GUILayout.Button("Run/Stop new simulation"))
        {
            var watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < gol.generations; i++)
            {
                gol.doStep(true);
            }
            watch.Stop();
            UnityEngine.Debug.LogWarning("Runtime: " + watch.ElapsedMilliseconds);
        }
    }
}
