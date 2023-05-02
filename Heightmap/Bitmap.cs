using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heightmap
{


    public class Bitmap
    {
        public readonly int Width;
        public readonly int Height;
        private readonly byte[] data;

        public Bitmap(int width, int height, byte[]? data = null)
        {
            Width = width;
            Height = height;

            if (data == null)
                this.data = new byte[width * height * 4];
            else
            {
                List<byte> dataList = data.ToList();
                ///<summary>
                ///We assume that this bitmape image use the header "BITMAPINFOHEADER"
                ///If you have any doubt, check https://en.wikipedia.org/wiki/BMP_file_format#DIB_header_(bitmap_information_header)
                ///</summary>
                dataList.RemoveRange(0, 54);

                this.data = dataList.ToArray();
            }
        }

        public void SetPixel(int x, int y, float value)
        {
            value *= 255;
            int offset = (Width * y + x) * 4;
            data[offset] = (byte)value;
            data[offset + 1] = (byte)value;
            data[offset + 2] = (byte)value;
        }



        public void SetPixels(float[,] data)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    SetPixel(x, y, data[x, y]);
        }




        public float GetPixelValue(int x, int y)
        {
            if (data != null)
            {
                int offset = (Width * y + x) * 4;

                byte r = data[offset];
                byte g = data[offset + 1];
                byte b = data[offset + 2];


                return (r + g + b) / 3f / 255f;

            }

            throw new InvalidDataException();
        }

        public float GetPixelValue(int x, int y, float scale)
        {
            float centerX = Width / 2f;
            float centerY = Height / 2f;

            float dx = (x - centerX) / scale + centerX;
            float dy = (y - centerY) / scale + centerY;

            int x1 = (int)dx;
            int y1 = (int)dy;

            int x2 = x1 + 1;
            int y2 = y1 + 1;

            float fracX = dx - x1;
            float fracY = dy - y1;

            if (x1 >= 0 && y1 >= 0 && x2 < Width && y2 < Height)
            {
                float topLeft = GetPixelValue(x1, y1);
                float topRight = GetPixelValue(x2, y1);
                float bottomLeft = GetPixelValue(x1, y2);
                float bottomRight = GetPixelValue(x2, y2);

                float top = topLeft + fracX * (topRight - topLeft);
                float bottom = bottomLeft + fracX * (bottomRight - bottomLeft);

                return top + fracY * (bottom - top);
            }

            return 0;
        }




        public byte[] GetBitmapImageData()
        {

            ///<summary>
            /// The size of the "bitmap file header" is 14 bytes and for the "bitmap info header" 40 , the total is therefore 54 bytes.
            ///This statement was made according to these sources
            ///https://en.wikipedia.org/wiki/BMP_file_format
            ///http://www.daubnet.com/en/file-format-bmp     
            ///</summary>
            ///

            //TODO: Faire en sorte de prendre en compte tous les type de bitmap
            const int bitmapHeaderSize = 54;

            byte[] imgBytesBuffer = new byte[data.Length + bitmapHeaderSize];
            imgBytesBuffer[0] = (byte)'B';
            imgBytesBuffer[1] = (byte)'M';

            //Size of the header in bytes (40 according to wikipedia)
            imgBytesBuffer[14] = 40;

            Array.Copy(BitConverter.GetBytes(imgBytesBuffer.Length), 0, imgBytesBuffer, 2, 4);
            Array.Copy(BitConverter.GetBytes(bitmapHeaderSize), 0, imgBytesBuffer, 10, 4);
            Array.Copy(BitConverter.GetBytes(Width), 0, imgBytesBuffer, 18, 4);
            Array.Copy(BitConverter.GetBytes(Height), 0, imgBytesBuffer, 22, 4);
            Array.Copy(BitConverter.GetBytes(32), 0, imgBytesBuffer, 28, 2);
            Array.Copy(BitConverter.GetBytes(data.Length), 0, imgBytesBuffer, 34, 4);
            Array.Copy(data, 0, imgBytesBuffer, bitmapHeaderSize, data.Length);

            return imgBytesBuffer;
        }





    }
}
