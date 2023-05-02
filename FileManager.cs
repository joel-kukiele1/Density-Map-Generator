using GalaxyShapeGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradientGenerator
{
    public static class FileManager
    {


        public static void SaveBitmapFile(Bitmap bitmap, string path)
        {
            byte[] bytes = bitmap.GetBitmapImageData();
            File.WriteAllBytes(path, bytes);
        }


    }
}
