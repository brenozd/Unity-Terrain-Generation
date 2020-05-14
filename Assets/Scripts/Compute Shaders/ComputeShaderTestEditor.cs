using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Threading;

[CustomEditor(typeof(ComputeShaderTest))]
public class ComputeShaderTestEditor : Editor
{
    ComputeShaderTest ct = null;

    void OnEnable()
    {
        ct = (ComputeShaderTest)target;
    }

    public override void OnInspectorGUI()
    {
        if (ct != null)
        {
            if (DrawDefaultInspector() && ct.autoUpdate)
            {
                ct.doStuff();
            }
            if (!ct.autoUpdate && GUILayout.Button("Generate new texture"))
            {
                ct.doStuff();
            }
        }

    }
}
