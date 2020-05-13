using UnityEngine;

public class ComputeShaderTest : MonoBehaviour
{
    public ComputeShader compute;
    
    [Header("Noise Texture settings")]
    public int size = 512;
    public Vector3 Offset = Vector3.zero;
    public float NoiseScale = 1f;

    [Header("Prefab settings")]
    public GameObject cpuPrefab;
    public GameObject gpuPrefab;

    private RenderTexture result;
    void Start()
    {
        //CPU Based Perlin Noise
        var watch = System.Diagnostics.Stopwatch.StartNew();

        NoiseTexture.Offset = Offset;
        NoiseTexture.NoiseScale = NoiseScale;
        Texture2D cpuTex = NoiseTexture.generateTexture2D(size, size);

        cpuPrefab.GetComponent<Renderer>().material.mainTexture = cpuTex;

        watch.Stop();
        Debug.Log("CPU Based: " + watch.ElapsedMilliseconds);


        //GPU Based Perlin Noise
        watch.Reset();
        watch = System.Diagnostics.Stopwatch.StartNew();

        int kernel = compute.FindKernel("NoiseTexture2D");
        result = new RenderTexture(size, size, 24);
        result.enableRandomWrite = true;
        result.format = RenderTextureFormat.ARGB32;
        result.Create();

        compute.SetTexture(kernel, "NoiseTexture", result);
        compute.SetFloat("noiseScale", NoiseTexture.NoiseScale);
        compute.SetInts("size", new int[2] { size, size });
        compute.SetFloats("offset", new float[3] {Offset.x, Offset.y, Offset.z});
        compute.Dispatch(kernel, size / 8, size / 8, 1);

        Texture2D gpuTex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        RenderTexture.active = result;
        gpuTex.ReadPixels(new Rect(0, 0, result.width, result.height), 0, 0);
        gpuTex.Apply();

        gpuPrefab.GetComponent<Renderer>().material.mainTexture = gpuTex;

        watch.Stop();
        Debug.Log("GPU Based: " + watch.ElapsedMilliseconds);
    }
}
