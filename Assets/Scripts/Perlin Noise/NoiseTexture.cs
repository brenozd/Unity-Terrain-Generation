using UnityEngine;
using UnityEngine.Rendering;

public class NoiseTexture : Noise
{
    ///<summary>
    ///Number that will multiply the coordinate to generate noiseCoordinate
    ///noiseCoord = coord / textureWidth * noiseScale
    ///</summary>
    public static float NoiseScale = 1;

    ///<summary>
    ///Vector 2 that will offset in texture coordinates
    ///</summary>
    public static Vector3 Offset = Vector3.zero;

    ///<summary> This function returns a Texture2D of Perlin Noise values
    ///<returns>
    ///Returns a Texture2D where each pixel is a ARGB made of noise Value in all components
    ///</returns>
    ///<param name = "TextureWidth">
    ///Width of texture
    ///</param>
    ///<param name = "TextureHeight">
    ///Height of texture
    ///</param>
    ///</summary>
    public static Texture2D generateTexture2D(int textureWidth, int textureHeight)
    {
        Texture2D noiseTexture = new Texture2D(textureWidth, textureHeight);
        for (int _x = 0; _x < textureWidth; _x++)
        {
            for (int _y = 0; _y < textureHeight; _y++)
            {
                float xCoord = (float)_x / textureWidth * NoiseScale + Offset.x;
                float yCoord = (float)_y / textureHeight * NoiseScale + Offset.y;
                float sample = PerlinNoise2D(xCoord, yCoord);
                Color noiseValue = new Color(sample, sample, sample);
                noiseTexture.SetPixel(_x, _y, noiseValue);
            }
        }
        noiseTexture.Apply();
        return noiseTexture;
    }

    ///<summary> This function returns a Texture3D of Perlin Noise values
    ///<returns>
    ///Returns a Texture3D where each pixel is a ARGB made of noise Value in all components
    ///</returns>
    ///<param name = "TextureWidth">
    ///Width of texture
    ///</param>
    ///<param name = "TextureHeight">
    ///Height of texture
    ///</param>
    ///<param name = "TextureDepth">
    ///Depth of texture
    ///</param>
    ///</summary>
    public static Texture3D generateTexture3D(int textureWidth, int textureHeight, int textureDepth)
    {
        Texture3D noiseTexture = new Texture3D(textureWidth, textureHeight, textureDepth, TextureFormat.ARGB32, false);
        noiseTexture.wrapMode = TextureWrapMode.Clamp;

        for (int _x = 0; _x < textureWidth; _x++)
        {
            for (int _y = 0; _y < textureHeight; _y++)
            {
                for (int _z = 0; _z < textureDepth; _z++)
                {
                    float xCoord = (float)_x / textureWidth * NoiseScale + Offset.x;
                    float yCoord = (float)_y / textureHeight * NoiseScale + Offset.y;
                    float zCoord = (float)_z / textureDepth * NoiseScale + Offset.z;

                    float sample = PerlinNoise3D(xCoord, yCoord, zCoord);
                    Color noiseColor = new Color(sample, sample, sample);
                    noiseTexture.SetPixel(_x, _y, _z, noiseColor);
                }
            }
        }
        noiseTexture.Apply();
        return noiseTexture;
    }

    //*************************
    //********GPU Based********
    //*************************
    ///<summary> This function returns a Texture3D of Perlin Noise values, generated in GPU
    ///<returns>
    ///Returns a Texture2D where each pixel is a ARGB made of noise Value in all components
    ///</returns>
    ///<param name = "TextureWidth">
    ///Width of texture
    ///</param>
    ///<param name = "TextureHeight">
    ///Height of texture
    ///</param>
    ///</summary>
    public static Texture2D generateTexture2D_GPU(int textureWidth, int textureHeight)
    {
        ComputeShader cs = (ComputeShader)Resources.Load("PerlinNoise");

        int kernel = cs.FindKernel("NoiseTexture2D");
        RenderTexture result = new RenderTexture(textureWidth, textureHeight, 24);
        result.enableRandomWrite = true;
        result.format = RenderTextureFormat.ARGB32;
        result.Create();

        ComputeBuffer buffer = new ComputeBuffer(textureWidth * textureHeight, sizeof(float));
        cs.SetBuffer(kernel, "noiseValues", buffer);
        cs.SetFloat("noiseScale", NoiseTexture.NoiseScale);
        cs.SetInts("size", new int[3] { textureWidth, textureHeight, 1 });
        cs.SetFloats("offset", new float[3] { Offset.x, Offset.y, Offset.z });
        cs.Dispatch(kernel, textureWidth / 16, textureHeight / 16, 1);

        Texture2D noiseTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
        float[] colors = new float[textureWidth*textureHeight];
        buffer.GetData(colors);
        buffer.Dispose();
        for (int i = 0; i < textureWidth; i++)
        {
            for (int j = 0; j < textureWidth; j++)
            {
                    float sample = colors[i*textureWidth + j];
                    Color color = new Color(sample, sample, sample, 1.0f);
                    noiseTexture.SetPixel(i, j, color);
            }
        }

        noiseTexture.Apply();
        
        return noiseTexture;
    }


    ///<summary> This function returns a Texture3D of Perlin Noise values, generated in GPU
    ///<returns>
    ///Returns a Texture3D where each pixel is a ARGB made of noise Value in all components
    ///</returns>
    ///<param name = "TextureWidth">
    ///Width of texture
    ///</param>
    ///<param name = "TextureHeight">
    ///Height of texture
    ///</param>
    ///<param name = "TextureDepth">
    ///Depth of texture
    ///</param>
    ///</summary>
    public static Texture3D generateTexture3D_GPU(int textureWidth, int textureHeight, int textureDepth)
    {
        ComputeShader cs = (ComputeShader)Resources.Load("PerlinNoise");

        int kernel = cs.FindKernel("NoiseTexture3D");

        ComputeBuffer buffer = new ComputeBuffer(textureWidth * textureHeight * textureDepth, sizeof(float));
        cs.SetBuffer(kernel, "noiseValues", buffer);
        cs.SetFloat("noiseScale", NoiseTexture.NoiseScale);
        cs.SetInts("size", new int[3] { textureWidth, textureHeight, textureDepth });
        cs.SetFloats("offset", new float[3] { Offset.x, Offset.y, Offset.z });
        cs.Dispatch(kernel, textureWidth / 8, textureHeight / 8, textureDepth / 8);

        Texture3D noiseTexture = new Texture3D(textureWidth, textureHeight, textureDepth, TextureFormat.ARGB32, false);
        float[] colors = new float[textureWidth*textureHeight*textureDepth];

        buffer.GetData(colors);

        for (int i = 0; i < textureWidth; i++)
        {
            for (int j = 0; j < textureWidth; j++)
            {
                for (int k = 0; k < textureWidth; k++)
                {
                    //See this
                    float sample = colors[i*textureWidth*textureHeight + j*textureHeight + k];
                    Color color = new Color(sample, sample, sample, 1.0f);
                    noiseTexture.SetPixel(i, j, k, color);
                }
            }
        }
        
        noiseTexture.Apply();
        buffer.Release();
        return noiseTexture;
    }
}
