using System.Collections.Generic;
using System;

public class Noise
{
    #region Private variables
    ///<summary>
    ///Seed used to generate noises.
    ///If its 0, a random seed will be generated
    ///</summary>
    public static int Seed = 0;

    //Random used to generate doubles
    private static System.Random rand;

    //List of pseudo random generated permutations (0 - 255)
    private static List<int> permutation = new List<int>();

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

    //Populate permutation table
    static void populatePermutationTable()
    {
        if (Seed == 0)
        {
            rand = new System.Random();
            Seed = rand.Next();
        }
        permutation.Clear();
        rand = new System.Random(Seed);
        for (int i = 0; i < 512; i++)
        {
            permutation.Add(rand.Next(256));
        }
    }

    public static void clearPermutationTable()
    {
        permutation.Clear();
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
        if (permutation.Count == 0)
        {
            populatePermutationTable();
        }
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

        if (permutation.Count == 0)
        {
            populatePermutationTable();
        }

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

        if (permutation.Count == 0)
        {
            populatePermutationTable();
        }

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
