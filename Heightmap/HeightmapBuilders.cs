using GalaxyShapeGenerator.Generator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Heightmap.Builders
{
    public enum GradientType
    {
        CircularGradient,
        LinearGradient,
        PerlinNoise
    }

    public static class HeightmapBuilders
    {
        private static readonly Noise noise = new Noise();


        public static float[,] GenerateHeightmap(int width, int height, GradientType gradient, float scale, int octaves, float persistence, float lacunarity, Vector2 offset, string bitmapMaskPath = "")
        {

            return gradient switch
            {
                GradientType.CircularGradient => CircularGradient(width, height, scale),
                GradientType.LinearGradient => LinearGradient(width, height),
                GradientType.PerlinNoise => SimplexNoise(width, height, scale, octaves, persistence, lacunarity, offset,bitmapMaskPath),
                _ => new float[0, 0],
            };
        }


  
        public static float[,] LinearGradient(int width, int height)
        {
            float[,] map = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {              
                    float value = x / (float)width;
                    map[x, y] = value;
                }
            }


            return map;
        }


        public static float[,] CircularGradient(int width, int height,float scale)
        {
            float[,] map = new float[width, height];

            int centerX = width /2;
            int centerY = width / 2;
            float sigma = scale;


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float value = MathF.Exp(-(MathF.Pow(x - centerX, 2) + MathF.Pow(y - centerY, 2)) / (2 * sigma * sigma));
                    map[x, y] = value;
                    
                }
            }

            return map;
        }


        public static float[,] SimplexNoise(int width, int height,float scale, int octaves, float persistence, float lacunarity, Vector2 offset,string bitmapMaskPath = "")
        {
            Bitmap? mask = new Bitmap(width, height);

            if (bitmapMaskPath != "")
            {
                mask = new Bitmap(width, height, ImageToBytes(bitmapMaskPath));
            }
            


            Random rnd = new Random(25565);
            Vector2[] octaveOffsets = new Vector2[octaves];

            for (int i = 0; i < octaves; i++)
            {
                float offsetX = rnd.Next(-10000, 10000) + offset.X;
                float offsetY = rnd.Next(-10000, 10000) + offset.Y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            float[,] map = new float[width, height];

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;



            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float frequency = 1;
                    float amplitude = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - width / 2) / scale * frequency + octaveOffsets[i].X ;
                        float sampleY = (y - height/2) / scale * frequency + octaveOffsets[i].Y ;

                        float value = noise.GetSimplexNoise(sampleX, sampleY);

                    
                        noiseHeight += value * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;

                    }

                    if(noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight) 
                        minNoiseHeight = noiseHeight;


                    map[x, y] = noiseHeight;


                }
            }



            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if(bitmapMaskPath == "")
                        map[x, y] = Normalize(minNoiseHeight, maxNoiseHeight, map[x, y]);
                    else
                        map[x, y] = mask.GetPixelValue(x,y) - Normalize(minNoiseHeight, maxNoiseHeight, map[x, y])/10;


                    map[x, y] = MathF.Max(map[x, y], 0);
                    map[x, y] = MathF.Min(map[x, y], 1);
                }
            }

            return map;
        }

        private static float Normalize(float min,float max, float value)
        {
            return (value - min) / (max - min);
        }



        private static byte[] ImageToBytes(string path)
        {
            if(!Directory.Exists(path))
                return null;

            return File.ReadAllBytes(path);
        }


    }
}
