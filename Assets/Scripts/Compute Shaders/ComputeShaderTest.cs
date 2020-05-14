using UnityEngine;

public class ComputeShaderTest : MonoBehaviour
{
    [Header("Noise Texture settings")]
    public int size = 512;
    public Vector3 Offset = Vector3.zero;
    public float NoiseScale = 1f;

    [Header("Prefab settings")]
    public GameObject cpuPrefab;
    public GameObject gpuPrefab;

    [Header("Auto update settings")]
    public bool autoUpdate = true;

    private RenderTexture result;
    private void Start() {
        doStuff();
    }
    public void doStuff()
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

        ComputeShader cs = (ComputeShader)Resources.Load("PerlinNoise");

        int kernel = cs.FindKernel("NoiseTexture2D");
        RenderTexture result = new RenderTexture(size, size, 24);
        result.enableRandomWrite = true;
        result.format = RenderTextureFormat.ARGB32;
        result.Create();

        ComputeBuffer buffer = new ComputeBuffer(size * size, sizeof(float));
        cs.SetBuffer(kernel, "noiseValues", buffer);
        cs.SetFloat("noiseScale", NoiseTexture.NoiseScale);
        cs.SetInts("size", new int[3] { size, size, 1 });
        cs.SetFloats("offset", new float[3] { Offset.x, Offset.y, Offset.z });
        cs.Dispatch(kernel, size / 16, size / 16, 1);

        Texture2D gpuTex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        float[] colors = new float[size*size];
        buffer.GetData(colors);
        buffer.Dispose();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                    float sample = colors[i*size + j];
                    Color color = new Color(sample, sample, sample, 1.0f);
                    gpuTex.SetPixel(i, j, color);
            }
        }

        gpuTex.Apply();

        gpuPrefab.GetComponent<Renderer>().material.mainTexture = gpuTex;

        watch.Stop();
        Debug.Log("GPU Based: " + watch.ElapsedMilliseconds);
    }
}
