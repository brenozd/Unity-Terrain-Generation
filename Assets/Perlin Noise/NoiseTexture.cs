using UnityEngine;

public class NoiseTexture : Noise
{
    ///<summary>
    ///Number that will multiply the coordinate to generate noiseCoordinate
    ///noiseCoord = coord / textureWidth * noiseScale
    ///</summary>
    public static float NoiseScale = 1;

    ///<summary> This function returns a Texture2D of Perlin Noise values
    ///<returns>
    ///Returns a Texture2D where each pixel is a RGB made of noise Value in all components
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
                float xCoord = (float)_x / textureWidth * NoiseScale;
                float yCoord = (float)_y / textureHeight * NoiseScale;
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
    ///Returns a Texture3D where each pixel is a RGB made of noise Value in all components
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
                    float xCoord = (float)_x / textureWidth * NoiseScale;
                    float yCoord = (float)_y / textureHeight * NoiseScale;
                    float zCoord = (float)_z / textureDepth * NoiseScale;

                    float sample = PerlinNoise3D(xCoord, yCoord, zCoord);
                    Color noiseColor = new Color(sample, sample, sample);
                    noiseTexture.SetPixel(_x, _y, _z, noiseColor);
                }
            }
        }
        noiseTexture.Apply();
        return noiseTexture;
    }

}
