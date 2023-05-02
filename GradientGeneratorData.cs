using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Numerics;

namespace GalaxyShapeGenerator.Generator
{
    public enum GradientType
    {
        LinearGradient,
        CircularGradient,
        PerlinNoise,
    }


    public static class GradientGeneratorData
    {


        public static float[,] GenerateGradient(int width, int height, GradientType gradient, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
        {

            switch (gradient)
            {
                case GradientType.CircularGradient:
                    return Gradient.CircularGradient(width, height, scale);
                case GradientType.LinearGradient:
                    return Gradient.LinearGradient(width, height);
                case GradientType.PerlinNoise:
                    return Gradient.SimplexNoise(width, height, scale, octaves, persistence, lacunarity, offset);
                default:
                    return new float[0, 0];
            }






        }


  



    }
}
