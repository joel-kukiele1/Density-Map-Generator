using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GalaxyShapeGenerator.Generator
{
    public static class Gradient
    {

        public static float[,] RectangularGradient(int width, int height)
        {
            float[,] map = new float[width, height];


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //float x = x / (float)width * 2 - 1;
                    //float y = y / (float)height * 2 - 1;
                    //float value = MathF.Max(MathF.Abs(x), MathF.Abs(y));



                    float x = i / (float)width *2;
                    float y = j / (float)height *2;

                    float value = MathF.Atan(x/y);

                    map[i, j] = value;
                }
            }


            return map;
        }

        public static float[,] LinearGradient(int width, int height)
        {
            float[,] map = new float[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {              
                    float value = i / (float)width;
                    map[i, j] = value;
                }
            }


            return map;
        }


        public static float[,] CircularGradient(int width, int height,float scale)
        {

            float[,] map = new float[width, height];

            //https://en.wikipedia.org/wiki/Gaussian_function
            //https://en.wikipedia.org/wiki/Gaussian_blur

            int centerX = width /2;
            int centerY = width / 2;
            float sigma = scale; //Zoom level


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

        public static float[,] CustomGradient(int width, int height)
        {
            int size = (width + height) / 2;

            float[,] map = new float[size, size];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Compute density based on distance from center
                    float distance = MathF.Sqrt((x - width / 2f) * (x - width / 2f) + (y - height / 2f) * (y - height / 2f));
                    float density = Math.Clamp(distance / (width / 2f),0,1);


                    map[x, y] = density;
                }
            }

            return map;
        }


        public static float[,] SimplexNoise(int width, int height,float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
        {
            //byte[] data = ImageToBytes(Path.Combine(Environment.CurrentDirectory, @"mask.bmp"));
            //Bitmap galaxyMask = new Bitmap(width, height, data);

            var noise = new Noise();

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

                    map[x, y] = /*mask.GetPixelValue(x,y,scale/100) -*/ Normalize(minNoiseHeight, maxNoiseHeight, map[x, y])/*/10*/;

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
            return File.ReadAllBytes(path);
        }







    }
}
