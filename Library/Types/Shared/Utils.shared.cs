/*

using System;
using System.Linq;

namespace PILSharp
{
    class Utils
    {
        internal static PILImageFormat GetImageFormat(byte[] bytes)
        {
            // BMP
            var bmp = new byte[] { 66, 77 };
            // Jpeg
            var jpeg = new byte[] { 255, 216, 255, 224 };
            // Jpeg Canon
            var jpeg2 = new byte[] { 255, 216, 255, 225 };
            // PNG
            var png = new byte[] { 137, 80, 78, 71 };

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
            {
                return PILImageFormat.Bmp;
            }

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
            {
                return PILImageFormat.Jpeg;
            }

            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
            {
                return PILImageFormat.Jpeg;
            }

            if (png.SequenceEqual(bytes.Take(png.Length)))
            {
                return PILImageFormat.Png;
            }
            
            throw new NotSupportedException($"Provided image format is not supported");
        }
    }
}

*/