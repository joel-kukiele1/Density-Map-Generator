﻿using Heightmap;
using Heightmap.Builders;
using System.Numerics;

namespace tests
{
    internal class Program
    {
        private static readonly int width = 511;
        private static readonly int height = 509;

        static void Main(string[] args)
        {
            Bitmap bmp = new(width, height);

            string bitmapMaskPath = Path.Combine(Environment.CurrentDirectory, @"masks/mask.bmp");

            float[,]  data = HeightmapBuilders.GenerateHeightmap(width, height, GradientType.PerlinNoise, 100, 16, 0.5f, 2, new Vector2(0, 0), bitmapMaskPath);
            bmp.SetPixels(data);



            string documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            SaveFile(bmp, @$"{documentsFolderPath}/heightmap_{DateTime.Now.ToShortDateString()}.bmp");


        }


        private static void SaveFile(Bitmap bitmap, string path)
        {
            File.WriteAllBytes(path, bitmap.GetBitmapImageData());
        }
    }
}