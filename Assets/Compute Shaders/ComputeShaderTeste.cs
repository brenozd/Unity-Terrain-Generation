
using UnityEngine;

public class ComputeShaderTeste : MonoBehaviour
{
    public int textRes = 256;
    public GameObject prefab;

    public ComputeShader compute;

    void Start()
    {
        RenderTexture Tex1_2D = new RenderTexture(textRes, textRes, 0, RenderTextureFormat.ARGB32);
        Tex1_2D.enableRandomWrite = true;
        Tex1_2D.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        Tex1_2D.volumeDepth = textRes;
        Tex1_2D.Create();

        int kernel = compute.FindKernel("NoiseGen");
        compute.SetTexture(kernel, "Noise1", Tex1_2D);
        compute.Dispatch(kernel, textRes / 8, textRes / 8, textRes / 8);
        prefab.GetComponent<Renderer>().material.SetTexture("_MainTex", Tex1_2D);
    }

    Texture2D convertRenderTexture2D(RenderTexture rt)
    {
        Texture2D tex2D = new Texture2D(textRes, textRes);
        RenderTexture.active = rt;
        tex2D.ReadPixels(new Rect(0, 0, textRes, textRes), 0, 0, false);
        tex2D.Apply();
        return tex2D;
    }
}
