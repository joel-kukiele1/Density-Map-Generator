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
        public readonly int width;
        public readonly int height;
        private readonly byte[] data;
        private readonly byte[] headerData;

        private const int BITMAPINFOHEADER_LENGTH  = 54;

        public Bitmap(int width, int height, byte[]? data = null)
        {
            this.width = width;
            this.height = height;
            headerData = new byte[BITMAPINFOHEADER_LENGTH];

            if (data == null)
                this.data = new byte[width * height * 4];
            else
            {
                for (int i = 0; i < BITMAPINFOHEADER_LENGTH; i++)
                    headerData[i] = data[i];

                List<byte> dataList = data.ToList();

                dataList.RemoveRange(0, BITMAPINFOHEADER_LENGTH);
                this.data = dataList.ToArray();
            }
        }


        public void SetPixel(int x, int y, float value)
        {
            value *= 255;
            int offset = (width * y + x) * 4;
            data[offset] = (byte)value;
            data[offset + 1] = (byte)value;
            data[offset + 2] = (byte)value;
        }



        public void SetPixels(float[,] data)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    SetPixel(x, y, data[x, y]);
        }




        public float GetPixelValue(int x, int y)
        {
            if (data != null)
            {
                int offset = (width * y + x) * 4;

                byte r = data[offset];
                byte g = data[offset + 1];
                byte b = data[offset + 2];


                return (r + g + b) / 3f / 255f;

            }

            throw new InvalidDataException();
        }

        public float GetPixelValue(int x, int y, float scale)
        {
            float centerX = width / 2f;
            float centerY = height / 2f;

            float dx = (x - centerX) / scale + centerX;
            float dy = (y - centerY) / scale + centerY;

            int x1 = (int)dx;
            int y1 = (int)dy;

            int x2 = x1 + 1;
            int y2 = y1 + 1;

            float fracX = dx - x1;
            float fracY = dy - y1;

            if (x1 >= 0 && y1 >= 0 && x2 < width && y2 < height)
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


        public int GetWidthFromHeader()
        {
            return headerData == null ? 0 : BitConverter.ToInt32(headerData.AsSpan()[18..22]);
        }

        public int GetHeightFromHeader()
        {
            return headerData == null ? 0 : BitConverter.ToInt32(headerData.AsSpan()[22..26]);
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


            //Bitmap file header
            byte[] imgBytesBuffer = new byte[data.Length + BITMAPINFOHEADER_LENGTH];
            imgBytesBuffer[0] = (byte)'B';
            imgBytesBuffer[1] = (byte)'M';
            Array.Copy(BitConverter.GetBytes(imgBytesBuffer.Length), 0, imgBytesBuffer, 2, 4);
            Array.Copy(BitConverter.GetBytes(BITMAPINFOHEADER_LENGTH), 0, imgBytesBuffer, 10, 4);

            //DIB header
            Array.Copy(BitConverter.GetBytes(40), 0, imgBytesBuffer, 14, 4); //Size of the header in bytes (40 according to wikipedia)
            Array.Copy(BitConverter.GetBytes(width), 0, imgBytesBuffer, 18, 4);
            Array.Copy(BitConverter.GetBytes(height), 0, imgBytesBuffer, 22, 4);
            Array.Copy(BitConverter.GetBytes(32), 0, imgBytesBuffer, 28, 2); //The number of bits per pixel, which is the color depth of the image.
            Array.Copy(BitConverter.GetBytes(data.Length), 0, imgBytesBuffer, 34, 4);

            Array.Copy(data, 0, imgBytesBuffer, BITMAPINFOHEADER_LENGTH, data.Length);

            return imgBytesBuffer;
        }





    }
}
