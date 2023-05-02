using GalaxyShapeGenerator;
using GalaxyShapeGenerator.Generator;
using System.Numerics;

namespace GradientGenerator
{
    internal class Program
    {
        private static int width = 512;
        private static int height = 512;
        private static float[,] data = new float[0,0];

        static void Main(string[] args)
        {
            Bitmap bmp = new(width, height);

            data = GradientGeneratorData.GenerateGradient(width, height, GradientType.PerlinNoise,30,4,0.5f,2, new Vector2(0,0));


            for (int x = 0; x < bmp.Width; x++)
                for (int y = 0; y < bmp.Height; y++)
                    bmp.SetPixel(x, y, data[x, y]);


            FileManager.SaveBitmapFile(bmp, @"C:\jnk\image\gradient.bmp");



 

        }
    }
}