using System.Collections.Generic;
using System;

public class Noise
{
    #region Private variables
    //List of pseudo random generated permutations (0 - 255)
    static List<int> permutation = new List<int>()
    {151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7,
    225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247,
    120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
    88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134,
    139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220,
    105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80,
    73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
    164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38,
    147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189,
    28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101,
    155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232,
    178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12,
    191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181,
    199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236,
    205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180};

    #endregion
    #region Math Functions
    //Cosine interpolation
    static double CosineInterpolation(double a, double b, double x)
    {
        double ft = x * Math.PI;
        double f = (1 - Math.Cos(ft)) * 0.5;
        return (a * (1 - f) + b * f);
    }

    //Linear interpolation
    static double LinearInterpolation(double a, double b, double x)
    {
        return a + x * (b - a);
    }

    //Fade equation provided by Perlin
    static double fade(double t)
    {
        //return t * t * (3 - 2 * t);
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    //Grad 1D
    static float Grad(int hash, float x)
    {
        return ((hash & 1) == 0 ? x : -x);
    }

    //Grad 2D
    static float Grad(int hash, float x, float y)
    {
        return ((hash & 1) == 0 ? x : -x) + ((hash & 2) == 0 ? y : -y);
    }

    //Grad 3D
    static float Grad(int hash, float x, float y, float z)
    {
        var h = hash & 15;
        var u = h < 8 ? x : y;
        var v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    #endregion
    #region Perlin Noise 1D
    ///<summary> This function returns a value of Perlin Noise in one dimension
    ///<returns>
    ///Returns a float between 0 and 1
    ///</returns>
    ///<param name = "x">
    ///Value of coordinate to calculate noise
    ///</param>
    ///</summary>
    public static float PerlinNoise1D(float x)
    {

        int _x = (int)x;

        int _g0 = permutation[_x & 255];
        int _g1 = permutation[(_x + 1) & 255];

        return (float)((LinearInterpolation(Grad(_g0, (x - _x)), Grad(_g1, ((x - _x) - 1)), fade(x - _x)) * 2) + 1) / 2;

    }

    ///<summary> This function returns a value of Perlin Noise in one dimension calculated by sum of multiples octaves
    ///<returns>
    ///Returns a float between 0 and 1
    ///</returns>
    ///<param name = "x">
    ///Value of coordinate to calculate noise
    ///</param>
    ///<param name = "Octaves">
    ///Number of octaves, to be generated
    ///</param>
    ///<param name = "Persistence">
    ///Number that determines how much each octave contributes to the overall shape. If your persistence is 1 all octaves contribute equally. If you persistence is more than 1 sucessive octaves contribute more
    ///</param>
    ///<param name = "Lacunarity">
    ///number that determines how much detail is added or removed at each octave. Lacunarity of more than 1 means that each octave will increase it’s level of fine grained detail. Lacunarity of 1 means that each octave will have the sam level of detail. Lacunarity of less than one means that each octave will get smoother
    ///</param>
    ///</summary>
    public static float PerlinNoise1D(float x, int octaves, float persistence = 0.25f, float lacunarity = 2)
    {
        float total = 0; //Sum of total values
        float frequency = 1; //Frequency of noise
        float amplitude = 1; //Amplitude of noise
        float maxValue = 0; //Sum of total amplitude
        //Iterate all octaves
        for (int i = 0; i < octaves; i++)
        {
            total += PerlinNoise1D(x * frequency) * amplitude; //Calculate an octave in an point
            maxValue += amplitude; //Sum amplitude
            amplitude *= persistence; //Recalculate amplitude
            frequency *= lacunarity; //Recalculate frequency
        }
        return total / maxValue; //Return normalized value
    }
    #endregion
    #region Perlin Noise 2D
    ///<summary> This function returns a value of Perlin Noise in two dimensions
    ///<returns>
    ///Returns a float between 0 and 1
    ///</returns>
    ///<param name = "X">
    ///Value of x coordinate to calculate noise
    ///</param>
    ///<param name = "Y">
    ///Value of y coordinate to calculate noise
    ///</param>
    ///</summary>
    public static float PerlinNoise2D(float x, float y)
    {
        //_pd0 = (x - _x, y - _y)
        //_pd1 = (x -_x + 1, y - _y)
        //_pd2 = (x - _x, y - _y +1)
        //_pd3 = (x - _x + 1, y - _y + 1)

        //Floor values of variables X and Y
        int _x = (int)x;
        int _y = (int)y;

        //Permutation values calculated with _pd0, _pd1, _pd2, _pd3
        int _g0 = permutation[((_x & 255) + (permutation[_y & 255])) & 255];
        int _g1 = permutation[(((_x + 1) & 255) + (permutation[_y & 255])) & 255];
        int _g2 = permutation[((_x & 255) + (permutation[(_y + 1) & 255])) & 255];
        int _g3 = permutation[(((_x + 1) & 255) + (permutation[(_y + 1) & 255])) & 255];

        //Iterpolations
        float a = (float)CosineInterpolation(Grad(_g0, (x - _x), (y - _y)), //Dot product between _g0 and _pd0
                                             Grad(_g1, (x - (_x + 1)), (y - _y)), //Dot product between _g1 and _pd1
                                             fade(x - _x)); //Fade function of _pd0.X

        float b = (float)CosineInterpolation(Grad(_g2, (x - _x), (y - (_y + 1))), //Dot product between _g2 and _pd2
                                             Grad(_g3, (x - (_x + 1)), (y - (_y + 1))), //Dot product between _g3 and _pd2
                                             fade(x - _x)); //Fade function of _pd0.X

        return (float)((LinearInterpolation(a, b, fade(y - _y)) + 1) / 2);
    }

    ///<summary> This function returns a value of Perlin Noise calculated in two dimensions by sum of multiples octaves
    ///<returns>
    ///Returns a float between 0 and 1
    ///</returns>
    ///<param name = "X">
    ///Value of x coordinate to calculate noise
    ///</param>
    ///<param name = "Y">
    ///Value of y coordinate to calculate noise
    ///</param>
    ///<param name = "Octaves">
    ///Number of octaves, to be generated
    ///</param>
    ///<param name = "Persistence">
    ///Number that determines how much each octave contributes to the overall shape. If your persistence is 1 all octaves contribute equally. If you persistence is more than 1 sucessive octaves contribute more
    ///</param>
    ///<param name = "Lacunarity">
    ///number that determines how much detail is added or removed at each octave. Lacunarity of more than 1 means that each octave will increase it’s level of fine grained detail. Lacunarity of 1 means that each octave will have the sam level of detail. Lacunarity of less than one means that each octave will get smoother
    ///</param>
    ///</summary>
    public static float PerlinNoise2D(float x, float y, int octaves, float persistence = 0.25f, float lacunarity = 2)
    {
        float total = 0; //Sum of total values
        float frequency = 1; //Frequency of noise
        float amplitude = 1; //Amplitude of noise
        float maxValue = 0; //Sum of total amplitude
        //Iterate all octaves
        for (int i = 0; i < octaves; i++)
        {
            total += PerlinNoise2D(x * frequency, y * frequency) * amplitude; //Calculate an octave in an point
            maxValue += amplitude; //Sum amplitude
            amplitude = (float)Math.Pow(persistence, i); //Recalculate amplitude
            frequency = (float)Math.Pow(lacunarity, i); //Recalculate frequency
        }
        return total / maxValue; //Return normalized value
    }
    #endregion
    #region Perlin Noise 3D
    ///<summary> This function returns a value of Perlin Noise in three dimensions
    ///<returns>
    ///Returns a float between 0 and 1
    ///</returns>
    ///<param name = "X">
    ///Value of x coordinate to calculate noise
    ///</param>
    ///<param name = "Y">
    ///Value of y coordinate to calculate noise
    ///</param>
    ///<param name = "Z">
    ///Value of z coordinate to calculate noise
    ///</param>
    ///</summary>
    public static float PerlinNoise3D(float x, float y, float z)
    {
        //_pd0 = (x - _x, y - _y, z - _z)
        //_pd1 = (x -_x + 1, y - _y, z - _z)
        //_pd2 = (x - _x, y - _y +1, z - _z)
        //_pd3 = (x - _x + 1, y - _y + 1, z - _z)
        //_pd4 = (x - _x, y - _y, z - _z + 1)
        //_pd5 = (x - _x + 1, y - _y, z - _z + 1)
        //_pd6 = (x - _x, y - _y + 1, z - _z + 1)
        //_pd7 = (x - _x + 1, y - _y + 1, z - _z + 1)

        //Floor values of variables X and Y
        int _x = (int)x;
        int _y = (int)y;
        int _z = (int)z;

        //Permutation values calculated with _pd0, _pd1, _pd2, _pd3
        int _g0 = permutation[(((_x) & 255) + (
                  permutation[((_y) & 255) +
                  permutation[(_z) & 255]])) & 255];

        int _g1 = permutation[(((_x + 1) & 255) + (
                  permutation[((_y) & 255) +
                  permutation[(_z) & 255]])) & 255];

        int _g2 = permutation[(((_x) & 255) + (
                  permutation[((_y + 1) & 255) +
                  permutation[(_z) & 255]])) & 255];

        int _g3 = permutation[(((_x + 1) & 255) + (
                  permutation[((_y + 1) & 255) +
                  permutation[(_z) & 255]])) & 255];

        int _g4 = permutation[(((_x) & 255) + (
                  permutation[((_y) & 255) +
                  permutation[(_z + 1) & 255]])) & 255];

        int _g5 = permutation[(((_x + 1) & 255) + (
                  permutation[((_y) & 255) +
                  permutation[(_z + 1) & 255]])) & 255];

        int _g6 = permutation[(((_x) & 255) + (
                  permutation[((_y + 1) & 255) +
                  permutation[(_z + 1) & 255]])) & 255];
                  
        int _g7 = permutation[(((_x + 1) & 255) + (
                  permutation[((_y + 1) & 255) +
                  permutation[(_z + 1) & 255]])) & 255];

        //Iterpolations
        float a = (float)CosineInterpolation(Grad(_g0, (x - _x), (y - _y), (z - _z)), //Dot product between _g0 and _pd0
                                             Grad(_g1, (x - (_x + 1)), (y - _y), (z - _z)), //Dot product between _g1 and _pd1
                                             fade(x - _x)); //Fade function of _pd0.X

        float b = (float)CosineInterpolation(Grad(_g2, (x - _x), (y - (_y + 1)), (z - _z)), //Dot product between _g2 and _pd2
                                             Grad(_g3, (x - (_x + 1)), (y - (_y + 1)), (z - _z)), //Dot product between _g3 and _pd2
                                             fade(x - _x)); //Fade function of _pd0.X

        float c = (float)CosineInterpolation(Grad(_g4, (x - _x), (y - _y), (z - (_z + 1))), //Dot product between _g4 and _pd4
                                             Grad(_g5, (x - (_x + 1)), (y - _y), (z - (_z + 1))), //Dot product between _g5 and _pd5
                                             fade(x - _x)); //Fade function of _pd0.X

        float d = (float)CosineInterpolation(Grad(_g6, (x - _x), (y - (_y + 1)), (z - (_z + 1))), //Dot product between _g6 and _pd6
                                             Grad(_g7, (x - (_x + 1)), (y - (_y + 1)), (z - (_z + 1))), //Dot product between _g7 and _pd7
                                             fade(x - _x)); //Fade function of _pd0.X

        float r = (float)CosineInterpolation(a, b, fade(y - _y)); //Interpolation between a and b
        float s = (float)CosineInterpolation(c, d, fade(y - _y)); //Interpolation between c and d

        return (float)((LinearInterpolation(r, s, fade(z - _z)) + 1) / 2);
    }

    ///<summary> This function returns a value of Perlin Noise calculated in three dimensions by sum of multiples octaves
    ///<returns>
    ///Returns a float between 0 and 1
    ///</returns>
    ///<param name = "X">
    ///Value of x coordinate to calculate noise
    ///</param>
    ///<param name = "Y">
    ///Value of y coordinate to calculate noise
    ///</param>
    ///<param name = "z">
    ///Value of y coordinate to calculate noise
    ///</param>
    ///<param name = "Octaves">
    ///Number of octaves, to be generated
    ///</param>
    ///<param name = "Persistence">
    ///Number that determines how much each octave contributes to the overall shape. If your persistence is 1 all octaves contribute equally. If you persistence is more than 1 sucessive octaves contribute more
    ///</param>
    ///<param name = "Lacunarity">
    ///number that determines how much detail is added or removed at each octave. Lacunarity of more than 1 means that each octave will increase it’s level of fine grained detail. Lacunarity of 1 means that each octave will have the sam level of detail. Lacunarity of less than one means that each octave will get smoother
    ///</param>
    ///</summary>
    public static float PerlinNoise3D(float x, float y, float z, int octaves, float persistence = 0.25f, float lacunarity = 2)
    {
        float total = 0; //Sum of total values
        float frequency = 1; //Frequency of noise
        float amplitude = 1; //Amplitude of noise
        float maxValue = 0; //Sum of total amplitude
        //Iterate all octaves
        for (int i = 0; i < octaves; i++)
        {
            total += PerlinNoise3D(x * frequency, y * frequency, z * frequency) * amplitude; //Calculate an octave in an point
            maxValue += amplitude; //Sum amplitude
            amplitude = (float)Math.Pow(persistence, i); //Recalculate amplitude
            frequency = (float)Math.Pow(lacunarity, i); //Recalculate frequency
        }
        return total / maxValue; //Return normalized value
    }
    #endregion
}
